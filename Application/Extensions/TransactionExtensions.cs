using Domain.Enum;
using Domain.Models;
using System.Text;

namespace Application.Extensions;
public static class TransactionExtensions
{
    public static Transaction CreateTransaction(this TransactionType type, long sendingAccount, long receivingAccount, long amount, bool status)
    {
        return new Transaction
        {
            SendingAccount = sendingAccount,
            ReceivingAccount = receivingAccount,
            Type = type,
            Amount = amount,
            Status = status
        };

    }

    public static Transaction CreateTransaction(this TransactionType type, long receivingAccount, long amount, bool status)
    {
        return new Transaction
        {
            ReceivingAccount = receivingAccount,
            Type = type,
            Amount = amount,
            Status = status
        };

    }

    //display transaction details as a multi-line string
    public static string DisplayTransaction(this Transaction transaction)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Transaction ID: {transaction.Id}");
        sb.AppendLine($"Type: {transaction.Type}");
        sb.AppendLine($"Sending Account: {(transaction.SendingAccount.HasValue ? transaction.SendingAccount.ToString() : "N/A")}");
        sb.AppendLine($"Receiving Account: {transaction.ReceivingAccount}");
        sb.AppendLine($"Amount: {transaction.Amount}");
        sb.AppendLine($"Status: {(transaction.Status ? "Completed" : "Pending")}");

        return sb.ToString();
    }
}