namespace FinancialMonitoring.Model
{
    public class Category : IDbObject
    {
        public Category(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}