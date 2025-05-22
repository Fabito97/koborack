namespace KoboRack.Utility
{
    public class SetWalletAccountNumber
    {
        public string SetWalletNumber(string phoneNumber)
        {
            if (phoneNumber.StartsWith("+234"))
            {
                phoneNumber = phoneNumber[4..];
                return phoneNumber;
            }
            else if (phoneNumber.StartsWith("0"))
            {
                phoneNumber = phoneNumber[1..];
                return phoneNumber;
            }
            else if (phoneNumber.Length == 10 && long.TryParse(phoneNumber, out long walletId))
            {
                phoneNumber = walletId.ToString();
                return phoneNumber;
            }
            else
            {
                throw new Exception("Invalid Phone Number format");
            }
        }
    }
}
