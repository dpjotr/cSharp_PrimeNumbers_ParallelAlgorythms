using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelLab2
{
          
    public enum Algorythm { ISPRIME, ERATHOSPHEN, MODIFIED_ERATHOSPHEN,
                                PARALLEL_ISPRIME_CYCLE, PARALLEL_ERATHOSPHEN11_DATA_DECOMPOSTION,
                                PARALLEL_ERATHOSPHEN12_CYCLE_DECOMPOSTION,
                                PARALLEL_ERATHOSPHEN2_PRIMEsSET_DECOMPOSTION,
                                TASK_POOL,
                                LOCK_SECTION

    }


    class Program
    {

        static void Main(string[] args)

        {

            for (int i = 950; i < 100000; i *= 10) F(i);
            Console.Read();

        }

        static void F(int n)
        {
            int N = n;
            Console.WriteLine($"Size of array------ {N}");
            Console.WriteLine("---------------Task_1");
            
            Console.WriteLine($"Looking for the primes from 0 to {N}");

            Primes p1 = new Primes(N);
            p1.Timer(Algorythm.ISPRIME);

            Primes p2 = new Primes(N);
            p2.Timer(Algorythm.ERATHOSPHEN);

            Primes p3 = new Primes(N);
            p3.Timer(Algorythm.MODIFIED_ERATHOSPHEN);

            Console.WriteLine("---------------Task_2");

            Primes p4 = new Primes(N);
            p4.Timer( Algorythm.PARALLEL_ISPRIME_CYCLE);

            Primes p5 = new Primes(N);
            p5.Timer(Algorythm.PARALLEL_ERATHOSPHEN11_DATA_DECOMPOSTION);

            Primes p6 = new Primes(N);
            p6.Timer( Algorythm.PARALLEL_ERATHOSPHEN12_CYCLE_DECOMPOSTION);

            Primes p7 = new Primes(N);
            p7.Timer(Algorythm.PARALLEL_ERATHOSPHEN2_PRIMEsSET_DECOMPOSTION);

            Console.WriteLine("---------------Task_3");

            Primes p8 = new Primes(N);
            p8.Timer(Algorythm.TASK_POOL);

            Console.WriteLine("---------------Task_4");

            Primes p9 = new Primes(N);
            p9.Timer( Algorythm.LOCK_SECTION);


            for (int i = 0; i < N; i++) 
                if (p1.res[i] != p2.res[i] || 
                    p1.res[i] != p3.res[i] || 
                    p1.res[i] != p4.res[i] ||
                    p1.res[i] != p5.res[i] ||
                    p1.res[i] != p6.res[i] ||
                    p1.res[i] != p7.res[i] ||
                    p1.res[i] != p8.res[i] ||
                    p1.res[i] != p9.res[i]) 
                    Console.WriteLine($@"Error found:  position {i} 
                                            ISPRIME                                           {p1.res[i]}  
                                            ERATHOSPHEN                                       {p2.res[i]}
                                            MODIFIED_ERATHOSPHEN                              {p3.res[i]}
                                            PARALLEL_ISPRIME_CYCLE                            {p4.res[i]}
                                            PARALLEL_ERATHOSPHEN11_DATA_DECOMPOSTION          {p5.res[i]}
                                            PARALLEL_ERATHOSPHEN12_CYCLE_DECOMPOSTION         {p6.res[i]}
                                            PARALLEL_ERATHOSPHEN2_PRIMEsSET_DECOMPOSTION      {p7.res[i]}
                                            TASK_POOL                                         {p8.res[i]}   
                                            LOCK_SECTION                                      {p9.res[i]} 
                    ");
            //p1.PrintPrimes();

          

 
        }
    }
}
