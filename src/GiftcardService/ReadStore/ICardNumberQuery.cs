namespace GiftcardService.ReadStore
{
    public interface ICardNumberQuery
    {
        bool IsAlreadyInUse(int cardNumber);
    }
}