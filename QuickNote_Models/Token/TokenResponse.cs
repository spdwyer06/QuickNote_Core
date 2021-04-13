using System;

namespace QuickNote_Models.Token
{
    public class TokenResponse
    {
        public string Token { get; set; }

        public DateTime IssuedAt { get; set; }

        public DateTime ExiresAt { get; set; }
    }
}