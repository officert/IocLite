using System;
using System.Collections.Generic;

namespace IocLite.SampleApp.Data
{
    public class ConsoleRepository : IConsoleRepository
    {
        public IEnumerable<Console> GetAllConsoles()
        {
            return new List<Console>
            {
                new Console
                {
                    Id = 1,
                    Name = ConsoleConstants.XboxOne
                },
                new Console
                {
                    Id = 2,
                    Name = ConsoleConstants.Xbox360
                },
                new Console
                {
                    Id = 3,
                    Name = ConsoleConstants.Playstation4
                },
                new Console
                {
                    Id = 4,
                    Name = ConsoleConstants.Playstation3
                },
                new Console
                {
                    Id = 5,
                    Name = ConsoleConstants.ThreeDS
                },
                new Console
                {
                    Id = 6,
                    Name = ConsoleConstants.PC
                },
                new Console
                {
                    Id = 7,
                    Name = ConsoleConstants.WiiU
                }
            };
        }
    }

    public interface IConsoleRepository
    {
        IEnumerable<Console> GetAllConsoles();
    }

    public class ConsoleConstants
    {
        public const string XboxOne = "XBox One";
        public const string Xbox360 = "XBox 360";
        public const string Playstation4 = "Playstation 4";
        public const string Playstation3 = "Playstation 3";
        public const string WiiU = "Wii U";
        public const string ThreeDS = "3DS";
        public const string PC = "PC";
    }
}