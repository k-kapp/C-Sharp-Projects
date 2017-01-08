using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO
{
    public class utils
    {
        /*
        public static void copyArrs<T>(T[] dest, T[] src)
        {
            for (int i = 0; i < (dest.Length < src.Length ? dest.Length : src.Length); i++)
            {
                dest[i] = src[i];
            }
        }
        */

        public static void copyArrs<T, U>(T[] dest, U[] src) where U: IConvertible
        {
            for (int i = 0; i < (dest.Length < src.Length ? dest.Length : src.Length); i++)
            {
                dest[i] = (T)Convert.ChangeType(src[i], typeof(T));
            }
        }
    }


    public class DimensionsNotEqualException : Exception
    {
        public DimensionsNotEqualException()
        {
        }

        public DimensionsNotEqualException(String message) : base(message)
        {
        }

    }

    public class LinearScale
    {
        public LinearScale(float minVal, float maxVal, float currVal, float incr)
        {
            this.currVal = currVal;
            this.minVal = minVal;
            this.maxVal = maxVal;
            this.incr = incr;
        }

        public void update()
        {
            currVal += incr;
            if (currVal > maxVal)
            {
                currVal = maxVal;
            }
            else if (currVal < minVal)
            {
                currVal = minVal;
            }
        }

        public static float operator * (LinearScale lhs, float rhs)
        {
            return rhs * lhs.currVal;
        }

        public static float operator * (LinearScale lhs, double rhs)
        {
            return (float)rhs * lhs.currVal;
        }

        public float currVal, minVal, maxVal, incr;
    }

    public class PSO<T>
    {
        private int dim;
        private int popSize;
        private double inertia;
        private LinearScale cogScale;
        private LinearScale socScale;
        private Func<double[], List<T>, double> objFunc;
        private int nbrRadius;
        private Random generator;
        private Tuple<double, double>[] ranges;
        private double [] globBestPos;
        private double globBestFitness;
        private float speed;

        private Individual<T>[] population;

        private float[,] optimPoints;
        private List<T> optimPosses;

        public PSO(int dim, int popSize, double inertia, LinearScale cogScale, LinearScale socScale, float speed,
            Func<double [], List<T>, double> objFunc, List<T> optimPosses, int nbrRadius, Tuple<double, double>[] ranges)
        {
            this.optimPosses = optimPosses;
            this.dim = dim;
            this.popSize = popSize;
            this.inertia = inertia;
            this.cogScale = cogScale;
            this.socScale = socScale;
            this.objFunc = objFunc;
            this.nbrRadius = nbrRadius;
            this.ranges = new Tuple<double, double>[dim];
            this.speed = speed;
            if (ranges.Length != dim)
            {
                throw new DimensionsNotEqualException("Number of dimensions must be equal to number of ranges");
            }
            Array.Copy(ranges, this.ranges, dim);

            generator = new Random();
            initPop();

            
        }

        public void resetBests()
        {
            foreach (var indiv in population)
            {
                indiv.resetBests();
            }
        }

        public void setInertia(float val)
        {
            inertia = val;
        }

        public float getInertia()
        {
            return (float)inertia;
        }

        public void setSpeed(float val)
        {
            speed = val;
        }

        public float getSpeed()
        {
            return speed;
        }

        public void setCogScale(float val)
        {
            cogScale.currVal = val;
        }

        public void setSocScale(float val)
        {
            socScale.currVal = val;
        }

        public void setSocIncr(float val)
        {
            socScale.incr = val;
        }

        public void setCogIncr(float val)
        {
            cogScale.incr = val;
        }

        public void initPop()
        {
            population = new Individual<T>[popSize];
            for (int i = 0; i < popSize; i++)
            {
                population[i] = new Individual<T>(this, i);
            }
        }

        public void updateAllPosses()
        {
            for (int i = 0; i < population.Length; i++)
            {
                population[i].updateVel();
                population[i].updatePos();
                population[i].evaluate();
            }
        }

        public void updateAllBests()
        {
            for (int i = 0; i < population.Length; i++)
            {
                population[i].updateBestNbr();
                population[i].updateBestPers();
            }
        }

        public void updateAll()
        {
            updateAllBests();
            updateAllPosses();
        }

        public void nextIteration()
        {
            updateAll();
            determineBest();
            cogScale.update();
            socScale.update();
        }

        public void optimise(int num)
        {
            for (int i = 0; i < num; i++)
            {
                nextIteration();
            }
        }

        public void determineBest()
        {
            double[] bestPos = new double[dim];
            double bestValue = -Double.MaxValue;

            for (int i = 0; i < popSize; i++)
            {
                if (population[i].getValue() > bestValue)
                {
                    bestValue = population[i].getValue();
                    bestPos = population[i].getPos();
                }
            }

            globBestPos = bestPos;
            globBestFitness = bestValue;
        }

        public Tuple<double [], double> getBest()
        {
            if (globBestPos == null)
                determineBest();
            return new Tuple<double[], double>(globBestPos, globBestFitness);
        }

        public double [,] yieldPosses()
        {
            double[,] allPosses = new double[popSize, dim];

            for (int i = 0; i < popSize; i++)
            {
                var pos = population[i].getPos();
                for (int j = 0; j < dim; j++)
                    allPosses[i, j] = pos[j];
            }
            return allPosses;
        }

        public float getCogScale()
        {
            return cogScale.currVal;
        }

        public float getSocScale()
        {
            return socScale.currVal;
        }


        class Individual<T>
        {
            private PSO<T> parentRef;
            private int nbrIdxLow;
            private int nbrIdxHigh;
            private double[] bestNbrPos;
            private double bestNbrValue;
            private double[] bestPersPos;
            private double bestPersValue;

            private double[] pos;
            private double[] vel;
            private double value;
            
            public Individual(PSO<T> parentRef, int thisIdx)
            {
                this.parentRef = parentRef;

                bestNbrPos = new double[parentRef.dim];
                bestPersPos = new double[parentRef.dim];

                bestNbrValue = -Double.MaxValue;
                bestPersValue = -Double.MaxValue;

                setNbrIdxes(thisIdx);

                initPos();
                initVel();
                evaluate();
            }

            public void resetBestPers()
            {
                Array.Copy(pos, bestNbrPos, pos.Length);
                bestPersValue = value;
            }

            public void resetBestNbr()
            {
                Array.Copy(pos, bestNbrPos, pos.Length);
                bestNbrValue = value;
            }

            public void resetBests()
            {
                resetBestPers();
                resetBestNbr();
            }

            public bool out_by(double distance)
            {
                var globBestPos = parentRef.getBest().Item1;

                double sum = 0.0;

                for (int i = 0; i < globBestPos.Length; i++)
                {
                    sum += (globBestPos[i] - pos[i]) * (globBestPos[i] - pos[i]);
                }

                return sum >= distance;
            }

            public void print_info()
            {
                
            }

            private void setNbrIdxes(int thisIdx)
            {
                nbrIdxLow = thisIdx - parentRef.nbrRadius;
                nbrIdxLow = nbrIdxLow < 0 ? parentRef.popSize - parentRef.nbrRadius : nbrIdxLow;

                nbrIdxHigh = thisIdx + parentRef.nbrRadius;
                nbrIdxHigh = nbrIdxHigh >= parentRef.popSize ? nbrIdxHigh - parentRef.popSize : nbrIdxHigh;
            }

            private void initPos()
            {
                pos = new double[parentRef.dim];
                for (int i = 0; i < pos.Length; i++)
                {
                    double dist = parentRef.ranges[i].Item2 - parentRef.ranges[i].Item1;
                    pos[i] = parentRef.generator.NextDouble() * dist + parentRef.ranges[i].Item1;
                }
            }

            private void initVel()
            {
                vel = new double[parentRef.dim];
                Random rand = new Random();

                for (int i = 0; i < vel.Length; i++)
                {
                    vel[i] = rand.NextDouble()*0.1;
                }
            }

            public void evaluate()
            {
                value = parentRef.objFunc(pos, parentRef.optimPosses);
            }

            public void updateBestNbr()
            {
                int i = nbrIdxLow;
                for (; i < parentRef.popSize; i++)
                {
                    if (parentRef.population[i].getValue() > bestNbrValue)
                    {
                        Array.Copy(parentRef.population[i].getPos(), bestNbrPos, parentRef.dim);
                        bestNbrValue = parentRef.population[i].getValue();
                    }
                }
            }

            public void updateBestPers()
            {
                if (getValue() >= bestPersValue)
                {
                    bestPersValue = getValue();
                    Array.Copy(getPos(), bestPersPos, parentRef.dim);
                }
            }

            public double [] getPos()
            {
                return pos;
            }

            public double getValue()
            {
                return value;
            }

            public void updatePos()
            {
                for (int i = 0; i < parentRef.dim; i++)
                {
                    pos[i] += vel[i];
                }
            }

            public void updateVel()
            {
                for (int i = 0; i < vel.Length; i++)
                {
                    vel[i] = vel[i] * parentRef.inertia 
                        + parentRef.cogScale * parentRef.speed * parentRef.generator.NextDouble() * (bestPersPos[i] - pos[i])
                        + parentRef.socScale * parentRef.speed * parentRef.generator.NextDouble() * (bestNbrPos[i] - pos[i]);
                }
            }
        }
    }
}
