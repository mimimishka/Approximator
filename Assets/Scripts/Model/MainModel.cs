using UniRx;

namespace Approximator
{
    public class MainModel
    {
        public delegate float Calculator(float x);
        public ReactiveDictionary<int, float> Table { get; private set; }
        public ReactiveProperty<Calculator> OLS { get; private set; }
        public ReactiveProperty<Calculator> Lagrange { get; private set; }

        public MainModel()
        {
            Table = new ReactiveDictionary<int, float>();
            OLS = new ReactiveProperty<Calculator>();
            OLS.Value = null;
            Lagrange = new ReactiveProperty<Calculator>();
            Lagrange.Value = null;
        }
        public void SetPoint(int x, float y)
        {
            if (Table.ContainsKey(x))
                Table.Remove(x);
            Table.Add(x, y);
        }
    }
}