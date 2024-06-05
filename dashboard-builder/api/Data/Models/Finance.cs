using System.ComponentModel.DataAnnotations;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Data.Models
{
    public class Finance
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string Account { get; set; } = string.Empty;

        public string AccountName { get; set; } = string.Empty;

        public string Amount { get; set; } = string.Empty;

        public string TransactionType { get; set; } = string.Empty;

        public string Currency { get; set; } = string.Empty;

        public string CreditCardNumber { get; set; } = string.Empty;

        public string CreditCardCvv { get; set; } = string.Empty;

        public string BitcoinAddress { get; set; } = string.Empty;

        public string EthereumAddress { get; set; } = string.Empty;

        public string RoutingNumber { get; set; } = string.Empty;

        public string Bic { get; set; } = string.Empty;

        public string Iban { get; set; } = string.Empty;

    }
}
