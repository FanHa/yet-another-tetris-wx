using Model.Tetri;

namespace Model.Reward
{
    public abstract class Reward
    {
        protected readonly TetrisFactory tetrisFactory = new TetrisFactory();
        public Item GenerateItem()
        {
            Tetri.Tetri tetri = CreateRandomTetri();
            FillCells(tetri);
            return new Item(GetName(), GetDescription(), tetri);
        }

        private Tetri.Tetri CreateRandomTetri()
        {
            return tetrisFactory.CreateRandomShape();
        }

        // Abstract method for subclasses to define how to fill cells
        protected abstract void FillCells(Tetri.Tetri tetri);

        protected abstract string GetName();
        protected abstract string GetDescription();
    }
}
