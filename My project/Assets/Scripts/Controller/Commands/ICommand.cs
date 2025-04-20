namespace Controller.Commands
{
    public interface ICommand
    {
        bool Execute();
        void Undo();
    }
}