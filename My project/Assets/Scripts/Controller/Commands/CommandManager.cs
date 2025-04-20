using System.Collections.Generic;

namespace Controller.Commands
{
    public class CommandManager
    {
        private readonly Stack<ICommand> commandStack = new Stack<ICommand>();

        public bool ExecuteCommand(ICommand command)
        {
            bool isSuccess = command.Execute();
            if (isSuccess)
            {
                commandStack.Push(command);
            }
            return isSuccess;
        }

        public void Undo()
        {
            if (commandStack.Count > 0)
            {
                ICommand command = commandStack.Pop();
                command.Undo();
            }
        }

        // 清空历史记录
        public void ClearHistory()
        {
            commandStack.Clear();
        }
    }
}