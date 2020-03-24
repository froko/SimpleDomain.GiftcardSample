namespace GiftcardService.Domain
{
    using System;

    public class GiftcardException : Exception
    {
        public GiftcardException(string message) : base(message)
        {
        }
    }
}