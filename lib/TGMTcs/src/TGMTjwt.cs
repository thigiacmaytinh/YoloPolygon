using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace TGMTcs
{
    public class TGMTjwt
    {        
		string m_secretKey = "thigiacmaytinh.com";
        SymmetricSecurityKey m_securityKey;

        static TGMTjwt instance = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public static TGMTjwt GetInstance()
        {
            if (instance == null)
                instance = new TGMTjwt();
            return instance;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public TGMTjwt()
        {
            m_securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(m_secretKey));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Init(string secretKey)
        {
            m_secretKey = secretKey;
            m_securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(m_secretKey));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateToken(string userId="anonymous", int numDays=7)
		{            
			var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                }),
                Expires = DateTime.UtcNow.AddDays(numDays),
				SigningCredentials = new SigningCredentials(m_securityKey, SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool ValidateToken(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			try
			{
                SecurityToken validatedToken;

                tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = false,
					ValidateAudience = false,
                    ValidateLifetime = true,
					IssuerSigningKey = m_securityKey
                }, out validatedToken);

                if (validatedToken.ValidTo < DateTime.UtcNow)
                    return false;

            }
			catch
			{
				return false;
			}
			return true;
		}
    }
}
