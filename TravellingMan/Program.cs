using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TravellingMan
{
    class Program
    {
        private static StreamReader reader;
        private static StreamWriter writer;
        static void Main(string[] args)
        {
            Program myProgram = new Program();
            myProgram.Run();
        }

        private void Run()
        {
            TcpClient Client = new TcpClient("jlab13.eal.dk", 11102);
            reader = new StreamReader(Client.GetStream());
            writer = new StreamWriter(Client.GetStream());
            Thread readerThread = new Thread(Read);
            readerThread.Start();
            while (Client.Connected)
            {
                
            }
        }

        private static void Read()
        {
            while (true)
            {
                List<string> stringList = reader.ReadLine()?.Split().ToList();
                if (stringList != null && stringList.Count == 3)
                {
                    Path newPath = new Path();
                    newPath.locationA = int.Parse(stringList[0]);
                    newPath.locationB = int.Parse(stringList[1]);
                    newPath.timeToWalk = int.Parse(stringList[2]);
                    PathRepository.Paths.Add(newPath);
                }
                if (stringList != null && stringList.Count == 2)
                {
                    Route route = new Route();
                    route.start = int.Parse(stringList[0]);
                    route.end = int.Parse(stringList[1]);
                    new Program().FindOptimalRoute(route);
                }
                stringList?.Clear();
            }
        }

        private void FindOptimalRoute(Route route)
        {
            int distanceWalked = 0;
            while (true)
            {
                distanceWalked++;
                List<Path> UseableAPaths = PathRepository.Paths.Where(x => x.locationA == route.start).Where(x => x.timeToWalk == distanceWalked).ToList();
                List<Path> UseableBPaths = PathRepository.Paths.Where(x => x.locationB == route.start).Where(x => x.timeToWalk == distanceWalked).ToList();

                foreach (var item in UseableAPaths)
                {
                    UseableBPaths.AddRange(PathRepository.Paths.Where(x => x.locationB == item.locationB && x.timeToWalk + item.startTime <= distanceWalked));
                    UseableAPaths.AddRange(PathRepository.Paths.Where(x => x.locationA == item.locationB && x.timeToWalk + item.startTime <= distanceWalked));
                }
                foreach (var item in UseableBPaths)
                {
                    UseableBPaths.AddRange(PathRepository.Paths.Where(x => x.locationB == item.locationA && x.timeToWalk + item.startTime <= distanceWalked));
                    UseableAPaths.AddRange(PathRepository.Paths.Where(x => x.locationA == item.locationA && x.timeToWalk + item.startTime <= distanceWalked));
                }
                foreach (var item in UseableAPaths)
                {
                    if (item.locationB == route.end)
                    {
                        Write(distanceWalked);
                    }
                    else if (item.timeToWalk >= distanceWalked + item.startTime)
                    {
                    }
                }
                foreach (var item in UseableBPaths)
                {
                    if (item.locationA == route.end)
                    {
                        Write(distanceWalked);
                    }
                    else if (item.timeToWalk >= distanceWalked + item.startTime)
                    {
                    }
                }

            }
        }

        private void Write(int distanceWalked)
        {
            //throw new NotImplementedException();
        }
    }
}
