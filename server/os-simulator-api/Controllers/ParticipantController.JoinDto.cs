using System;

namespace SomeSimulator.Controllers
{
    public class JoinDto
    {
        public string SessionGuid { get; set; }
        public TimeSpan Duration { get; set; }
    }
}