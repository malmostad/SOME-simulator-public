namespace SoMeSimulator.Data.Models.Abstract
{
    interface ICommand <T> where T : EntityBase
    {
        void Execute(T entity);
        bool Validate (T entity);
    }
}
