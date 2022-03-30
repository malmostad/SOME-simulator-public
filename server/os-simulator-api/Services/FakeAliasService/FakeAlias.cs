using Bogus;

namespace SomeSimulator.Services.FakeAliasService
{
    public class FakeAlias: IFakeAlias
    {
        
        public string GenerateAlias()
        {
            return new Person("sv").UserName;
        }
    }
}