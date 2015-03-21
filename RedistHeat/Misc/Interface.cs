// ReSharper disable All
namespace RedistHeat
{
	public interface ITempExchangable
	{
		bool Validate();
		void ExchangeHeat();
	}

	public interface IXml
	{
		void Xmlize();
	}
}
