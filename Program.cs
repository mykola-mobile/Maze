using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Threading; // нужно для использования sleep()
using System.Media;

namespace Maze
{
    class Program
    {

        enum Direction                    //  Enum-чегЪ для управления ГГ
        {
            Stay,
            Up,
            Down,
            Left,
            Right
        }

        // Символы рисования лабиринта
        const int LTC = 9484;   // Left top corner     -   верхний левый угол           Код=9484 Символ=┌
        const int LBC = 9492;   // Left bottom corner  -   нижний левый угол            Код=9492 Символ=└
        const int RTC = 9488;   // Right top corner    -   верхний правый угол          Код=9488 Символ=┐
        const int RBC = 9496;   // Right bottom corner -   нижний правый угол           Код=9496 Символ=┘
        const int TT = 9516;   // Top T               -   верхний Т                    Код=9516 Символ=┬
        const int BT = 9524;   // Bottom T            -   нижний Т                     Код=9524 Символ=┴
        const int LT = 9500;   // Left T              -   левый Т                      Код=9500 Символ=├
        const int RT = 9508;   // Right T             -   правый Т                     Код=9508 Символ=┤
        const int C = 9532;   // Cross               -   крест                        Код=9532 Символ=┼
        const int V = 9474;   // Vertical line       -   вертикальная линия           Код=9474 Символ=│
        const int H = 9472;   // Horizontal line     -   горизонтальная линия         Код=9472 Символ=─

        const int GG = 9787;   // Protagonist sign    -   символ Главного Героя - Слава Герою ! :-)     Код=9787 Символ=☻
        const int FS = 183;    // Footstep sign       -   символ следов главного героя                  Код=183 Символ=·

        const int JG = 9675;   // JumpGate sign       -   символ врат гиперперехода          Код=9675 Символ=○
        const int ES = 32;     // Empty space         -   символ пустого места               Код=32 Символ=' '
        const int MS = 83;     // Maze start          -   символ S - признак начала лабиринтега
        const int ME = 69;     // Maze end            -   символ E - признак конца лабиринтега

        const int Y_OFFSET = 2; // смещение по y для отображения лабиринтега
        const int X_OFFSET = 3; // смещение по х для отображения лабиринтега

        const int LAB_X = 74;    // размерность лабиринта по оси х 
        const int LAB_Y = 20;    // размерность лабиринта по оси y 

        static int Hero_y;        //  координата у положения ГГ
        static int Hero_x;        //  координата x положения ГГ

        static bool userNotWantToExit = true;
        static bool levelComplete = false;
        static int level = 1;     // Текущий уровень игры

        //static int[,] maze = { { LTC,H,H,H,TT,H,H,H,H,RTC},                 // масив, в котором будет лежать лабиринтегЪ
        //                    { MS,ES,ES,ES,V,ES,ES,ES,ES,V},
        //                    { LT,H,RTC,ES, LBC,H,H,RTC,ES,V},
        //                    { V,ES,V,ES,ES,ES,ES,V,ES,V},
        //                    { V,ES,LBC,H,H,RTC,ES,V,ES,V},
        //                    { V,ES,ES,ES,ES,V,ES,V,ES,V},
        //                    { LT,H,RTC,ES,ES,V,ES,V,ES,V},
        //                    { V,ES,V,ES,ES,V,ES,V,ES,V},
        //                    { V,ES,ES,ES,ES,ES,ES,ES,ES,ME},
        //                    { LBC,H,BT,H,H,BT,H,H,H,RBC} };

        // масив, в котором будет лежать лабиринтегЪ - Уровень 1
        static int[,] maze = {  { LTC,H,H,H,H,H,H,H,TT,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,TT,H,TT,H,H,H,H,H,H,H,H,TT,H,H,H,TT,H,H,H,TT,H,H,H,RTC},
                                { MS,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V},
                                { V,ES,V,ES,V,ES,V,ES,V,ES,LTC,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,TT,H,RTC,ES,V,ES,LBC,RTC,ES,H,H,H,H,RTC,ES,V,ES,V,ES,V,ES,V,ES,V,ES,V,ES,V},
                                { V,ES,V,ES,V,ES,V,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,V,ES,V,ES,V,ES,ES,ES,V,ES,V,ES,V,ES,V},
                                { V,ES,V,ES,V,ES,V,ES,V,ES,LBC,H,RTC,ES,LTC,H,H,H,H,H,H,H,ES,H,H,H,H,H,H,H,TT,H,H,H,TT,H,H,H,H,H,H,H,H,H,RTC,ES,V,ES,LBC,H,H,RTC,ES,LBC,H,H,H,RTC,ES,V,ES,LBC,H,H,H,RTC,ES,V,ES,ES,ES,V,ES,V },
                                { V,ES,LBC,H,BT,H,RT,ES,V,ES,ES,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,ES,ES,LBC,H,BT,H,RTC,ES,V,ES,V},
                                { V,ES,ES,ES,ES,ES,V,ES,LBC,H,RTC,ES,V,ES,V,ES,V,ES,V,ES,LTC,H,H,H,H,H,H,H,RTC,ES,V,ES,V,ES,V,ES,LTC,H,H,H,H,H,RTC,ES,V,ES,LBC,H,H,RTC,ES,LBC,H,H,H,RTC,ES,V,ES,LBC,H,RTC,ES,V,ES,ES,ES,ES,ES,LBC,H,RT,ES,V},
                                { LT,H,H,H,RTC,ES,V,ES,ES,ES,V,ES,V,ES,V,ES,V,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,V,ES,V,ES,V,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,V,ES,LT,H,H,H,RTC,ES,ES,ES,V,ES,V },
                                { V,ES,ES,ES,V,ES,LT,H,H,ES,V,ES,V,ES,V,ES,V,ES,V,ES,V,ES,LTC,H,H,ES,RTC,ES,V,ES,LT,H,RT,ES,V,ES,V,ES,H,H,RTC,ES,V,ES,LT,H,H,RTC,ES,LBC,H,H,H,RTC,ES,V,ES,LBC,H,RTC,ES,V,ES,V,ES,ES,ES,LBC,H,RTC,ES,V,ES,V },
                                { V,ES,V,ES,V,ES,V,ES,ES,ES,V,ES,V,ES,V,ES,V,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,V,ES,V,ES,V,ES,V,ES,V,ES,ES,ES,V,ES,V,ES,V,ES,ES,V,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,V,ES,V,ES,V,ES,V,ES,ES,ES,V,ES,V,ES,V },
                                { V,ES,V,ES,ES,ES,V,ES,LTC,H,RBC,ES,LBC,H,RBC,ES,V,ES,LT,H,H,H,RBC,ES,LTC,H,RBC,ES,V,ES,V,ES,V,ES,V,ES,LBC,H,RTC,ES,V,ES,V,ES,LT,H,ES,LT,H,H,H,RTC,ES,V,ES,LBC,H,H,ES,V,ES,V,ES,V,ES,V,ES,V,ES,V,ES,ES,ES,V },
                                { LT,H,BT,H,H,H,RBC,ES,V,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,V,ES,ES,ES,V,ES,LBC,H,RBC,ES,V,ES,ES,V,ES,ES,ES,V,ES,V,ES,ES,ES,ES,ES,V,ES,V,ES,V,ES,V,ES,V,ES,LBC,H,H,H,RT },
                                { V,ES,ES,ES,ES,ES,ES,ES,V,ES,LTC,H,H,H,H,H,RBC,ES,V,ES,LTC,H,H,H,RBC,ES,LTC,H,BT,H,H,H,RBC,ES,LBC,H,RTC,ES,V,ES,ES,ES,ES,ES,V,ES,LTC,RBC,ES,V,ES,V,ES,LT,H,H,H,H,H,RT,ES,V,ES,V,ES,V,ES,V,ES,ES,ES,ES,ES,V },
                                { V,ES,H,H,H,H,H,H,RBC,ES,V,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,LBC,H,RBC,ES,H,H,H,H,RBC,ES,V,ES,ES,V,ES,LT,H,RBC,ES,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,V,ES,LT,H,TT,H,RTC,ES,V },
                                { V,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,LTC,H,H,H,H,H,RBC,ES,V,ES,V,ES,LTC,H,RBC,ES,LTC,H,RTC,ES,LTC,H,RTC,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,LTC,RBC,ES,V,ES,ES,ES,LTC,H,RTC,ES,V,ES,LTC,H,RTC,ES,V,ES,V,ES,V,ES,V,ES,V },
                                { LT,H,H,H,TT,H,H,H,TT,H,RBC,ES,V,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,ES,ES,V,ES,V,ES,V,ES,LBC,H,H,H,H,H,H,H,H,H,RTC,ES,ES,ES,LBC,H,H,RBC,ES,LTC,H,RBC,ES,V,ES,V,ES,V,ES,V,ES,V,ES,V,ES,V,ES,V,ES,V },
                                { V,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,LTC,H,TT,H,H,H,RT,ES,LT,H,H,H,TT,H,RBC,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,V,ES,V,ES,V,ES,V,ES,V },
                                { V,ES,V,ES,V,ES,V,ES,V,ES,LTC,H,RBC,ES,V,ES,V,ES,ES,ES,V,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,LBC,H,H,H,H,H,H,H,H,H,H,ES,V,ES,H,H,H,H,H,H,H,BT,H,H,H,H,H,H,H,RBC,ES,V,ES,LBC,H,RBC,ES,V,ES,V,ES,V },
                                { V,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,ES,ES,V,ES,ES,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,V,ES,ME },
                                { LBC,H,BT,H,H,H,BT,H,H,H,BT,H,H,H,BT,H,H,H,BT,H,H,H,H,H,BT,H,H,H,BT,H,BT,H,H,H,H,H,H,H,H,H,H,H,H,H,BT,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,BT,H,H,H,H,H,BT,H,BT,H,RBC }
                            };
        // масив, в котором будет лежать лабиринтегЪ - Уровень 2
        static int[,] maze2 = { { ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, },
                                { ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, },
                                { ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, },
                                { ES,ES,ES,LTC,H,H,H,H,H,H,H,H,H,H,TT,H,H,H,H,H,H,H,H,H,TT,H,TT,H,TT,H,H,H,H,H,H,H,TT,H,H,H,H,H,H,H,H,H,H,H,TT,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,RTC,ES,ES,ES},
                                { ES,ES,ES,MS,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES},
                                { ES,ES,ES,V,ES,V,ES,V,ES,H,H,H,RTC,ES,V,ES,LTC,H,H,H,H,H,H,ES,V,ES,V,ES,V,ES,LTC,H,H,H,H,ES,V,ES,LTC,H,H,H,H,H,H,H,H,ES,V,ES,LTC,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,ES,LTC,RTC,ES,V,ES,ES,ES },
                                { ES,ES,ES,V,ES,V,ES,V,ES,ES,ES,ES,V,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,V,ES,V,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,LT,RBC,ES,V,ES,ES,ES},
                                { ES,ES,ES,V,ES,V,ES,LBC,H,H,RTC,ES,V,ES,V,ES,V,ES,LTC,H,H,H,H,H,RBC,ES,V,ES,V,ES,V,ES,H,H,H,H,RBC,ES,V,ES,LTC,H,H,H,TT,H,H,H,RBC,ES,LBC,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,H,RT,ES,ES,V,ES,ES,ES},
                                { ES,ES,ES,V,ES,V,ES,ES,ES,ES,V,ES,V,ES,V,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,H,RT,ES,ES,ES},
                                { ES,ES,ES,V,ES,LBC,H,H,H,H,BT,H,BT,H,RBC,ES,V,ES,V,ES,LTC,H,H,H,H,H,RBC,ES,V,ES,LT,H,H,H,H,H,H,H,RBC,ES,V,ES,V,ES,ES,ES,LTC,H,H,ES,V,ES,V,ES,V,ES,V,ES,V,ES,LTC,H,TT,H,TT,RTC,ES,V,ES,ES,V,ES,ES,ES},
                                { ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,V,ES,V,ES,ES,ES,V,ES,V,ES,LBC,H,BT,TT,RBC,ES,V,ES,V,ES,LBC,RT,ES,LT,H,ES,V,ES,ES,ES},
                                { ES,ES,ES,LT,H,H,H,H,H,H,H,H,H,ES,H,H,BT,H,RBC,ES,V,ES,H,TT,H,H,H,H,RBC,ES,V,ES,H,H,H,H,H,H,H,H,RBC,ES,V,ES,V,ES,V,ES,LTC,H,RT,ES,V,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,ES,V,ES,ES,ES},
                                { ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,V,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,V,ES,V,ES,V,ES,LBC,H,H,RTC,ES,LBC,H,H,H,H,H,H,H,RT,ES,V,ES,H,RT,ES,ES,ES},
                                { ES,ES,ES,V,ES,LTC,H,TT,H,TT,H,TT,H,TT,H,TT,H,TT,H,TT,BT,RTC,ES,V,ES,LTC,H,H,TT,H,BT,H,TT,H,H,H,TT,H,H,H,TT,H,RBC,ES,V,ES,V,ES,ES,ES,V,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,V,ES,ES,V,ES,ES,ES},
                                { ES,ES,ES,V,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,LBC,H,RBC,ES,V,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,LBC,H,H,H,BT,H,H,RTC,ES,LBC,H,H,H,H,H,H,H,H,H,RT,ES,LBC,H,H,RT,ES,ES,ES},
                                { ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,ES,ES,LT,RTC,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,ES,ES,V,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,ES,V,ES,ES,ES,ES,ME,ES,ES,ES},
                                { ES,ES,ES,LBC,H,BT,H,BT,H,BT,H,BT,H,BT,H,BT,H,BT,H,BT,H,H,H,H,H,BT,BT,H,H,H,BT,H,H,H,BT,H,H,H,BT,H,H,H,BT,H,BT,H,H,H,H,H,H,H,H,BT,H,H,H,H,H,H,H,H,H,H,H,BT,H,H,H,H,RBC,ES,ES,ES} ,
                                { ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, },
                                { ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, },
                                { ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, ES, },

                              };
        const string Message1 = "    Ваша задача - управляя Самым Главным Героемпри помощи  клавиш  управления курсором (вверх,вниз, вправо, влево) пройти лабиринт. Выйти вы можете всегда при помощи клавиши ESC. Удачи !";
        const string Message2 = "   Ценой потери шаровар, оберштандартеншарфюрерЗлобной Киевской Хунты Тарас \"Москаляку-на-гилляку\"  Пэррэбiйнiс-Майданськiй  выкрал  в  битыхМолью  восточных  регионах  артефакт страшеннойсилы:  Рецепт \"Сало в шоколаде\".  Артефакт надодоставить  Злобнейшей Киевской Хунте,  лично  вруки Рейхсфюреру Конфеткину.                      Но места незнакомые...";
        const string Message3 = "     Тарас смог  пробраться  незамеченным черезнезнакомую территорию.                              Однако - дальше линия столкновения и постыорков-аболченцев...                                 Как перейти на украинскую сторону ?...";
        const string Message4 = "    Решение есть - идти через канализацию!         Сорвав ближайший \"аквафреш\", намотав его напалку и смочив в бензине, Тарас получил факел.     Жаль, что он светит очень недалеко...";
        const string Message5 = "    Наконец-то Тарас выбрался к своим !            Предстоит отнести артефакт в Киев,  но  этосовсем другая история.... ";
        static void Main(string[] args)
        {

            MyPreparation();
            //level = 2;
            //Hero_x = 4;
            //Hero_y = 4;
            //maze2[Hero_y, Hero_x] = GG;  //  ставим ГГ на игровую доску
            do
            {
                DrawMaze();
                MakeYouMove();

                if (maze[Hero_y, Hero_x + 1] == ME)
                {
                    level = 2;
                    Hero_x = 4;
                    Hero_y = 4;
                    maze2[Hero_y, Hero_x] = GG;  //  ставим ГГ на игровую доску
                    userNotWantToExit = false;
                }


            } while (userNotWantToExit);

            CoolWriter(17, 12, 47, Message3);
            Console.ReadKey();
            userNotWantToExit = true;
            DrawBox(0, 0, 79, 24, "", " Уровень 2 ");
            CoolWriter(17, 12, 47, Message4);
            Console.ReadKey();
            do
            {
                DrawMaze();
                MakeYouMove();

                if (maze2[Hero_y, Hero_x + 1] == ME)
                {
                    userNotWantToExit = false;
                }
              

            } while (userNotWantToExit);

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            CoolWriter(17, 12, 47, Message5);
            Console.ReadKey();
        }

        static void MyPreparation()
        {
            Console.CursorVisible = false;  //дабы глаз не раздражал (особенно левый)
            Console.OutputEncoding = Encoding.UTF8; // надо для а)отображения непечатаемых символов, б)чтобы везде работало одинаково 
            Console.Title = "Maze - Лабиринтег";    // красотень!    :-)
            Console.SetWindowSize(80, 25);          // типа - любимый размерчег еще с ДОСа
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;


            Hero_y = 1;   // задаем первоначальные координаты ГГ в лабиринтеге,
            Hero_x = 1;   // причем - используя индексы массива
            maze[Hero_y, Hero_x] = GG;  //  ставим ГГ на игровую доску
            DrawBox(0, 0, 79, 24, " .NET15-2 Матвейчук Н. Дом.работа на enum-чек, строки и область видимости ", " Нажмите любую клавишу для начала игры... ");
            Console.SetCursorPosition(2, 2); Console.WriteLine(" @@@@@          @@@@@");
            Console.SetCursorPosition(2, 3); Console.WriteLine("@@@@@@@        @@@@@@@");
            Console.SetCursorPosition(2, 4); Console.WriteLine("@@@@@@@        @@@@@@@");
            Console.SetCursorPosition(2, 5); Console.WriteLine("@@@@@@@@      @@@@@@@@                  Л а б и р и н т е г Ъ");
            Console.SetCursorPosition(2, 6); Console.WriteLine("@@@@@@@@      @@@@@@@@");
            Console.SetCursorPosition(2, 7); Console.WriteLine("@@@@@@@@      @@@@@@@@");
            Console.SetCursorPosition(2, 8); Console.WriteLine("@@@@ @@@@    @@@@ @@@@        @@@@@@@       @@@@@@@@@@@@@       @@@@@@@");
            Console.SetCursorPosition(2, 9); Console.WriteLine("@@@@ @@@@    @@@@ @@@@      @@@@@@@@@@@    @@@@@@@@@@@@@@@     @@@@@@@@@@");
            Console.SetCursorPosition(2, 10); Console.WriteLine("@@@@ @@@@    @@@@ @@@@     @@@@@@@@@@@@     @@@@@@@@@@@@@@   @@@@@@@@@@@@@");
            Console.SetCursorPosition(2, 11); Console.WriteLine("@@@@  @@@@  @@@@  @@@@     @@@      @@@@            @@@@@   @@@@       @@@@");
            Console.SetCursorPosition(2, 12); Console.WriteLine("@@@@  @@@@  @@@@  @@@@     @@       @@@@           @@@@@    @@@@       @@@@");
            Console.SetCursorPosition(2, 13); Console.WriteLine("@@@@  @@@@  @@@@  @@@@            @@@@@@          @@@@@     @@@@       @@@@");
            Console.SetCursorPosition(2, 14); Console.WriteLine("@@@@   @@@  @@@   @@@@         @@@@@@@@@         @@@@@      @@@@@@@@@@@@@@@");
            Console.SetCursorPosition(2, 15); Console.WriteLine("@@@@   @@@@@@@@   @@@@      @@@@@@@@@@@@        @@@@@       @@@@@@@@@@@@@@");
            Console.SetCursorPosition(2, 16); Console.WriteLine("@@@@   @@@@@@@@   @@@@     @@@@@    @@@@       @@@@@        @@@@");
            Console.SetCursorPosition(2, 17); Console.WriteLine("@@@@   @@@@@@@@   @@@@    @@@@      @@@@      @@@@@         @@@@");
            Console.SetCursorPosition(2, 18); Console.WriteLine("@@@@    @@@@@@    @@@@    @@@@     @@@@@    @@@@@            @@@@      @@@@");
            Console.SetCursorPosition(2, 19); Console.WriteLine("@@@@    @@@@@@    @@@@    @@@@    @@@@@@   @@@@@@@@@@@@@@    @@@@@    @@@@");
            Console.SetCursorPosition(2, 20); Console.WriteLine("@@@@    @@@@@@    @@@@    @@@@@@@@@@@@@@@  @@@@@@@@@@@@@@@    @@@@@@@@@@@");
            Console.SetCursorPosition(2, 21); Console.WriteLine("@@@@     @@@@     @@@@     @@@@@@@@  @@@@  @@@@@@@@@@@@@@@     @@@@@@@@@");
            Console.SetCursorPosition(2, 22); Console.WriteLine(" @@       @@       @@        @@@@     @@     @@@@@@@@@@@@        @@@@@");

            Console.ReadKey();

            CoolWriter(17, 12, 47, Message1);
            Console.ReadKey();
            DrawBox(0, 0, 79, 24, "", " Уровень 1 ");
            CoolWriter(17, 12, 47, Message2);
            Console.ReadKey();
        }


        static void CoolWriter(int startXPosition, int startYPosition, int lineLenght, string Message)    // поиграемся со string
        {
            int currentCharToDisplay = 0;   // текущая буква из Message для отображения
            int x_offset = 0;               // текущее смещение по х-су для отображения 
            int lineNumber = 0;             // текущее смещение по у-ку для отображения
            int windowYSize;                // для расчета высоты окна сообщения

            if ((Message.Length % lineLenght) != 0)   // если есть остаток, значит надо еще одна строка для вывода  
            {
                windowYSize = (Message.Length / lineLenght) + 1;
            }
            else
            {
                windowYSize = (Message.Length / lineLenght);
            }

            DrawBox(startXPosition - 2, startYPosition - 2, startXPosition + lineLenght + 1, startYPosition + windowYSize + 1, "", " Любая клавиша - продолжить ");
            do
            {
                for (int i = 0; i <= 2; i++)    // для украсивления вывода делаем эффект плавного появления (черный-серый-белый) символов
                {
                    Console.SetCursorPosition(startXPosition + x_offset, startYPosition + lineNumber);
                    switch (i)
                    {
                        case 0:
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case 1:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case 2:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                    }

                    Console.Write(Message[currentCharToDisplay]);
                    Thread.Sleep(15);

                }
                x_offset++; // следующий символ выведем в следующую по х позицию
                currentCharToDisplay++;  //смещаемся по строке символов
                if (x_offset == lineLenght) //если достигли заказонной длины строки, то переходим на следующую строку с первой позиции
                {
                    x_offset = 0;
                    lineNumber++;
                }
            } while (currentCharToDisplay < Message.Length);
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void DrawMaze()
        {
            if (level == 1)
            {
                for (int y = 0; y < LAB_Y; y++)
                {
                    for (int x = 0; x < LAB_X; x++)
                    {
                        Console.SetCursorPosition(x + X_OFFSET, y + Y_OFFSET);
                        if (maze[y, x] == GG)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;  //  ГГ у нас чиста жЁлты
                        }
                        if (maze[y, x] == FS)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkMagenta; //  гадит за собой ГГ исключительно пурпуром
                        }
                        if ((maze[y, x] == MS) | (maze[y, x] == ME))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;  // это у нас служебные коды, игроку их видеть не нать
                        } 

                        Console.Write((char)maze[y, x]);
                        Console.ForegroundColor = ConsoleColor.White;   //  вертаем цвет назад
                    }
                }
            }
            
            if (level==2)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Black;
                for (int  y = 0; y < LAB_Y; y++)
                {
                    Console.ForegroundColor = ConsoleColor.Black;


                    for (int x = 0; x < LAB_X; x++)
                    {

                        if ((Hero_y - y > 4) | (Hero_x - x > 4))            // В этих 3-х if мы создаем "Эффект освещенности"
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                        }

                        if (((Hero_y - y <= 4) & (Hero_y - y >= -4)) & ((Hero_x - x <= 4) & (Hero_x - x >= -4)))
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }

                        if (((Hero_y - y <= 3) & (Hero_y - y >= -3)) & ((Hero_x - x <= 3) & (Hero_x - x >= -3)))
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                        

                        Console.SetCursorPosition(x + X_OFFSET, y + Y_OFFSET);
                        if (maze2[y, x] == GG)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;  //  ГГ у нас чиста жЁлты
                        }
                        if (maze2[y, x] == FS)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkMagenta; //  гадит за собой ГГ исключительно пурпуром
                        }
                        if (maze2[y, x] == MS | maze2[y, x] == ME)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;  // это у нас служебные коды, игроку их видеть не нать
                        }

                        Console.Write((char)maze2[y, x]);
                        Console.ForegroundColor = ConsoleColor.Black;   //  вертаем цвет назад
                    }
                    Console.ForegroundColor = ConsoleColor.Black;   //  вертаем цвет назад
                }
            }
        }
    

        static void MakeYouMove()
        {
            Direction move;
            move = Direction.Stay;

            ConsoleKeyInfo enteredKey;
            enteredKey = Console.ReadKey();

            if (enteredKey.Key == ConsoleKey.Escape)    // игрок нажал ESC и жаждет выйти
            {
                userNotWantToExit = false;
                return;
            }

            if (enteredKey.Key == ConsoleKey.UpArrow)
            {
                move = Direction.Up;
            }

            if (enteredKey.Key == ConsoleKey.DownArrow)
            {
                move = Direction.Down;
            }

            if (enteredKey.Key == ConsoleKey.LeftArrow)
            {
                move = Direction.Left;
            }

            if (enteredKey.Key == ConsoleKey.RightArrow)
            {
                move = Direction.Right;
            }

            if (level == 1)
                {
                switch (move)   // радостно юзаем enum-чегЪ
                {
                    case Direction.Stay:
                        break;
                    case Direction.Up:
                        if (CheckMyMove(move))  // проверяем, что ход допустим
                       
                        {
                            maze[Hero_y, Hero_x] = ES;  // убираем ГГ с игровой доски
                            if (maze[Hero_y - 1, Hero_x] != FS)     //  данная проверка нужна, чтобы корректно убирать следы ГГ
                            {
                                maze[Hero_y, Hero_x] = FS;
                            }
                            maze[Hero_y - 1, Hero_x] = GG;  // устанавливаем новое положение ГГ
                            Hero_y--;                     // корректируем координаты ГГ
                        }
                        break;
                    case Direction.Down:
                       
                        if (CheckMyMove(move))
                        {
                            maze[Hero_y, Hero_x] = ES;
                            if (maze[Hero_y + 1, Hero_x] != FS)
                            {
                                maze[Hero_y, Hero_x] = FS;
                            }
                            maze[Hero_y + 1, Hero_x] = GG;
                            Hero_y++;
                        }
                        break;
                    case Direction.Left:
                       
                        if (CheckMyMove(move))
                        {
                            maze[Hero_y, Hero_x] = ES;
                            if (maze[Hero_y, Hero_x - 1] != FS)
                            {
                                maze[Hero_y, Hero_x] = FS;
                            }
                            maze[Hero_y, Hero_x - 1] = GG;
                            Hero_x--;
                        }
                        break;
                    case Direction.Right:
                       
                        if (CheckMyMove(move))
                        {
                            maze[Hero_y, Hero_x] = ES;
                            if (maze[Hero_y, Hero_x + 1] != FS)
                            {
                                maze[Hero_y, Hero_x] = FS;
                            }
                            maze[Hero_y, Hero_x + 1] = GG;
                            Hero_x++;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (level == 2)
            {
                switch (move)   // радостно юзаем enum-чегЪ
                {
                    case Direction.Stay:
                        break;
                    case Direction.Up:
                        if (CheckMyMove(move))  // проверяем, что ход допустим
                        {
                            maze2[Hero_y, Hero_x] = ES;  // убираем ГГ с игровой доски
                            if (maze2[Hero_y - 1, Hero_x] != FS)     //  данная проверка нужна, чтобы корректно убирать следы ГГ
                            {
                                maze2[Hero_y, Hero_x] = FS;
                            }
                            maze2[Hero_y - 1, Hero_x] = GG;  // устанавливаем новое положение ГГ
                            Hero_y--;                     // корректируем координаты ГГ
                        }
                        break;
                    case Direction.Down:
                        if (CheckMyMove(move))
                        {
                            maze2[Hero_y, Hero_x] = ES;
                            if (maze2[Hero_y + 1, Hero_x] != FS)
                            {
                                maze2[Hero_y, Hero_x] = FS;
                            }
                            maze2[Hero_y + 1, Hero_x] = GG;
                            Hero_y++;
                        }
                        break;
                    case Direction.Left:
                        if (CheckMyMove(move))
                        {
                            maze2[Hero_y, Hero_x] = ES;
                            if (maze2[Hero_y, Hero_x - 1] != FS)
                            {
                                maze2[Hero_y, Hero_x] = FS;
                            }
                            maze2[Hero_y, Hero_x - 1] = GG;
                            Hero_x--;
                        }
                        break;
                    case Direction.Right:
                        if (CheckMyMove(move))
                        {
                            maze2[Hero_y, Hero_x] = ES;
                            if (maze2[Hero_y, Hero_x + 1] != FS)
                            {
                                maze2[Hero_y, Hero_x] = FS;
                            }
                            maze2[Hero_y, Hero_x + 1] = GG;
                            Hero_x++;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        static bool CheckMyMove(Direction move)
        {
            if (level == 1)
            {
                switch (move)   // очень радостно юзаем enum-чегЪ и радуемся примеру из области видимости переменных (move тут и в MakeYouMove())
                {
                    case Direction.Stay:
                        {
                            return false;
                        }

                    case Direction.Up:
                        {
                            if (maze[Hero_y - 1, Hero_x] == ME)
                            {
                                levelComplete = true;
                                return false;
                            }

                            if (maze[Hero_y - 1, Hero_x] == ES | maze[Hero_y - 1, Hero_x] == FS)   // проверяем, чтобы в указанном направлении не было препятствий, свой след не препятствие
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        }

                    case Direction.Down:
                        if (maze[Hero_y + 1, Hero_x] == ME)
                        {
                            levelComplete = true;
                            return false;
                        }
                        if (maze[Hero_y + 1, Hero_x] == ES | maze[Hero_y + 1, Hero_x] == FS)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    case Direction.Left:
                        if (maze[Hero_y, Hero_x - 1] == ME)
                        {
                            levelComplete = true;
                            return false;
                        }
                        if (maze[Hero_y, Hero_x - 1] == ES | maze[Hero_y, Hero_x - 1] == FS)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    case Direction.Right:
                        if (maze[Hero_y, Hero_x + 1] == ME)
                        {
                            levelComplete = true;
                            return false;
                        }
                        if (maze[Hero_y, Hero_x + 1] == ES | maze[Hero_y, Hero_x + 1] == FS)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    default:
                        return false;
                }
            }
            else
            {
                switch (move)   // очень радостно юзаем enum-чегЪ и радуемся примеру из области видимости переменных (move тут и в MakeYouMove())
                {
                    case Direction.Stay:
                        {
                            return false;
                        }

                    case Direction.Up:
                        {
                            if (maze2[Hero_y - 1, Hero_x] == ME)
                            {
                                levelComplete = true;
                                return false;
                            }

                            if (maze2[Hero_y - 1, Hero_x] == ES | maze2[Hero_y - 1, Hero_x] == FS)   // проверяем, чтобы в указанном направлении не было препятствий, свой след не препятствие
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        }

                    case Direction.Down:
                        if (maze2[Hero_y + 1, Hero_x] == ME)
                        {
                            levelComplete = true;
                            return false;
                        }
                        if (maze2[Hero_y + 1, Hero_x] == ES | maze2[Hero_y + 1, Hero_x] == FS)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    case Direction.Left:
                        if (maze2[Hero_y, Hero_x - 1] == ME)
                        {
                            levelComplete = true;
                            return false;
                        }
                        if (maze2[Hero_y, Hero_x - 1] == ES | maze2[Hero_y, Hero_x - 1] == FS)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    case Direction.Right:
                        if (maze2[Hero_y, Hero_x + 1] == ME)
                        {
                            levelComplete = true;
                            return false;
                        }
                        if (maze2[Hero_y, Hero_x + 1] == ES | maze2[Hero_y, Hero_x + 1] == FS)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    default:
                        return false;
                }
            }
        }

        static void DrawBox(int y, int x, int y1, int x1, string Mess, string Podval)     // Процедура рисования рамки, с очисткой экрана внутри рамки и выводом заголовка посередине рамки вверху
        {                                                               // или рисования рамки с текстом вопроса в заголовке и полем для ввода значения и возвратом введенного значения
            int i, ii;                                                  //     

            for (i = x; i <= x1; i++)                                   // Чистим облать экрана (заполняем пробелом)
            {
                for (ii = y; ii <= y1; ii++)
                {
                    Console.SetCursorPosition(ii, i);
                    Console.Write(" ");
                }
            }
            for (i = x; i <= x1; i++)                                   //  Рисуем вертикальные линии
            {
                Console.SetCursorPosition(y, i);
                Console.Write("║");
                Console.SetCursorPosition(y1, i);
                Console.Write("║");
            }
            for (i = y; i <= y1; i++)                                   // Рисуем горизонтальные линии
            {
                Console.SetCursorPosition(i, x);
                Console.Write("═");
                Console.SetCursorPosition(i, x1);
                Console.Write("═");
            }
            Console.SetCursorPosition(y, x);                            // Расставляем уголки по местам
            Console.Write("╔");
            Console.SetCursorPosition(y, x1);
            Console.Write("╚");
            Console.SetCursorPosition(y1, x);
            Console.Write("╗");
            Console.SetCursorPosition(y1, x1);
            Console.Write("╝");

            i = Mess.Length / 2;                                          // Получаем длину строки и вычисляем координаты для вывода
            ii = (y1 - y) / 2 - i + y;
            Console.SetCursorPosition(ii, x);                           // Выводим заголовок
            Console.Write(Mess);

            i = Podval.Length;
            if (i > 75)
            {
                Console.SetCursorPosition(2, x1);
            }
            else
            {
                Console.SetCursorPosition(y1 - i - 3, x1);
            }
            Console.Write(Podval);
        }


    }
    
}
