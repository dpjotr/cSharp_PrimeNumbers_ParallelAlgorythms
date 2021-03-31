using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace ParallelLab2
{
    public class Primes
    {
        int capacity;
        internal int[] source;
        internal bool[] res;
        internal int[] primes;
        int threads;
        List<int> ErList;
        

        public Primes(int n)
        {
            threads = 3; //System.Environment.ProcessorCount-4;
            capacity = n;
            source = Enumerable.Range(0,n+1).ToArray();
            res = new bool[source.Length];
            ErList = new List<int>();
            for (int i = 2; i < res.Length; i++) res[i] = !res[i];
            

        }

        internal void FindPrimes(Algorythm a)
        {
            switch (a)
            {
                case Algorythm.ISPRIME:  SimplestSearch(); break;
                case Algorythm.ERATHOSPHEN: Erathosphen(capacity); break;
                case Algorythm.MODIFIED_ERATHOSPHEN: ModifiedErathosphen(capacity); break;
                case Algorythm.PARALLEL_ISPRIME_CYCLE: ParallelIsPrimeCycle(capacity); break;
                case Algorythm.PARALLEL_ERATHOSPHEN11_DATA_DECOMPOSTION: ParallelErathospen1_DataDecomposition(capacity); break;
                case Algorythm.PARALLEL_ERATHOSPHEN12_CYCLE_DECOMPOSTION: ParallelErathospen_DataDecompositionCycle(capacity); break;
                case Algorythm.PARALLEL_ERATHOSPHEN2_PRIMEsSET_DECOMPOSTION: ParallelErathospen_PrimesSetDecomposition(capacity); break;
                case Algorythm.TASK_POOL: TaskPool(capacity); break;
                case Algorythm.LOCK_SECTION: LockSection(capacity); break;
            }
            
        }
        void LockSection(int N)
        {
            int sqrN = (int)Math.Sqrt(N);
            Erathosphen(sqrN);

            Thread[] thr = new Thread[threads];
            int current_index = 0;
         
            Object thisLock = new Object(); 

            for (int thread = 0; thread < threads; thread++)
            {   
                thr[thread] = new Thread(() =>
                {
                    while (true)
                    {
                        if (current_index >= ErList.Count)
                            break;
                        int currentPrime;
                        lock (thisLock){ currentPrime = ErList[current_index++];  }

                        for(int candidate = sqrN; candidate < N; candidate++)
                              if (candidate % currentPrime == 0)  res[candidate] = false;                                                 
                    }  
                });
                thr[thread].Start();
            }
            foreach (var x in thr) x.Join();
        }


    

        void TaskPool(int N)
        {
            int sqrN = (int)Math.Sqrt(N);
            Erathosphen(sqrN);

            ManualResetEvent[] events =new ManualResetEvent[ErList.Count];
                // Добавляем в пул рабочие элементы с параметрами
                for (int p = 0; p < ErList.Count; p++)
                {
                    events[p] = new ManualResetEvent(false);
                    ThreadPool.QueueUserWorkItem(Run, new object[] { ErList[p], events[p] });
                }
                // Дожидаемся завершения
                WaitHandle.WaitAll(events);   
        }

        void Run(object o)
        {
            int bP = (int)((object[])o)[0];
            ManualResetEvent ev = ((object[])o)[1] as ManualResetEvent;
            int n = (int)Math.Sqrt(res.Length);
            for (int k = n; k < res.Length; k++)
                if (k % bP == 0)  res[k] = false;

            ev.Set();
        }
        void ParallelErathospen_PrimesSetDecomposition(int N)
        {
            int sqrN = (int)Math.Sqrt(N);
            Erathosphen(sqrN);

            int primesForThread = ErList.Count/threads;

            if (primesForThread < 1)
            {
                for (int k = sqrN; k < res.Length; k++)
                    for (int j = 0; j < ErList.Count; j++)
                        if (k % ErList[j] == 0) { res[k] = false; break; }
            }
            else
            {
                Thread[] thr = new Thread[threads];
                for (int thread = 0; thread < threads; thread++)
                {
                    int i = thread;
                    thr[thread] = new Thread(() =>
                    {                        
                        for (int j = i; j<ErList.Count; j+=threads)                            
                                for (int k = sqrN; k < res.Length; k++)
                                    if(res[k])
                                        if (k % ErList[j] == 0) { res[k] = false; continue; }
                    });

                    thr[thread].Start();
                }
                foreach (var x in thr) x.Join();
            }
        }
        void ModifiedErathosphen(int N)
        {
            int sqrN = (int)Math.Sqrt(N);
            Erathosphen(sqrN);
            for (int k = sqrN; k < res.Length; k++)
                for (int j = 0; j < ErList.Count; j++)
                      if (k % ErList[j] == 0) {res[k] = false; break; }          
        }


        void ParallelIsPrimeCycle(int N)
        {
            Thread[] thr = new Thread[threads];
            for (int thread = 0; thread < threads; thread++)
            {
                int i = thread;
                thr[i] = new Thread(() =>
                {
                    for (int k = i; k < source.Length; k += threads) 
                        if (!IsPrime(source[k])) res[k] = false;
                });

                thr[i].Start();                
            }        
            foreach (var x in thr) x.Join();
        }

        //Data decomposition
        void ParallelErathospen1_DataDecomposition(int N)
        {
            int sqrN = (int)Math.Sqrt(N);
            Erathosphen(sqrN);

            int searchSize = (N - sqrN) / threads;
            Thread[] thr = new Thread[threads];

            for (int i = 0; i < threads; i++)
            {
                int start = sqrN + i * searchSize;
                int finish = sqrN + (i+1)  * searchSize + 1;

               thr[i] = new Thread(()=>
                {
                    for (int k = start; (k < finish) && (k < N); k++)
                        for (int j = 0; j < ErList.Count; j++)
                            if (k % ErList[j] == 0) { res[k] = false; break; } 
                });
                thr[i].Start();
            }
            foreach (var x in thr) x.Join();
        }

        void ParallelErathospen_DataDecompositionCycle(int N)
        {
            int sqrN = (int)Math.Sqrt(N);
            Erathosphen(sqrN);
                        
            Thread[] thr = new Thread[threads];

            for (int i = 0; i < threads; i++)
            {
                int tempI = i;

                thr[tempI] = new Thread(() =>
                {
                    for (int k = sqrN+ tempI; k < N; k += threads)
                        for (int j = 0; j  < ErList.Count; j++)
                            if (k % ErList[j] == 0) { res[k] = false; break; }
                });
                thr[i].Start();
            }
            foreach (var x in thr) x.Join();
        }


        void SimplestSearch()
        {

            for (int i = 0; i < res.Length; i++)
            {
                if (!IsPrime(source[i])) res[i] = false;
            }
        }

        void Erathosphen(int N)
        {
            
            int n = (int)Math.Sqrt(N);            

            for (int k = 2, m =source[k]; m<=n; k++, m=source[k])
            {
                if (res[k] == false) continue;

                for(int j = m*m; j < N; j++)
                {                    
                    if (source[j] % m == 0 ) res[j] = false;
                }                
            }

            for (int i = 0; i < N; i++)
                if (res[i]) ErList.Add(source[i]);
            
        }




        bool IsPrime(int candidate)
        {
            // Test whether the parameter is a prime number.
            if ((candidate & 1) == 0)
                            if (candidate == 2) return true;
                            else return false;
            

            for (int i = 3; (i * i) <= candidate; i += 2)
                            if ((candidate % i) == 0) return false;
                        
            return candidate != 1 ;
        }

        public void PrintPrimes() { int i = 0; foreach (var x in primes) Console.WriteLine((++i).ToString()+"-- "+x); }

        internal void Timer( Algorythm A)
        {
            DateTime dt1, dt2;
            dt1 = DateTime.Now;
            FindPrimes(A);
            dt2 = DateTime.Now;

            String intro = "";
            switch (A)
            {
                case Algorythm.ISPRIME:
                    intro = "ISPRIME"; break;
                case Algorythm.ERATHOSPHEN:
                    intro = "ERATHOSPHEN"; break;
                case Algorythm.MODIFIED_ERATHOSPHEN:
                    intro = "MODIFIED_ERATHOSPHEN"; break;
                case Algorythm.PARALLEL_ISPRIME_CYCLE:
                    intro = "PARALLEL_ISPRIME_CYCLE"; break;
                case Algorythm.PARALLEL_ERATHOSPHEN11_DATA_DECOMPOSTION:
                    intro = "PARALLEL_ERATHOSPHEN11_DATA_DECOMPOSTION"; break;
                case Algorythm.PARALLEL_ERATHOSPHEN12_CYCLE_DECOMPOSTION:
                    intro = "PARALLEL_ERATHOSPHEN12_CYCLE_DECOMPOSTION"; break;
                case Algorythm.PARALLEL_ERATHOSPHEN2_PRIMEsSET_DECOMPOSTION:
                    intro = "PARALLEL_ERATHOSPHEN2_PRIMEsSET_DECOMPOSTION"; break;
                case Algorythm.TASK_POOL:
                    intro = "TASK_POOL"; break;
                case Algorythm.LOCK_SECTION:
                    intro = "LOCK_SECTION"; break;



            }
            primes = source.Where(x => res[Array.IndexOf(source, x)] == true).ToArray();
            Console.WriteLine($@"{(dt2 - dt1).TotalMilliseconds: 0000.0000} ms took {intro}   ");

        }
    }
}
