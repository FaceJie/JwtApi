using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi.Models;

namespace WebApi.Content.Jwt
{
    /// <summary>
    /// token帮助类
    /// </summary>
    public class TokenHelper<T>where T :class
    {
        /// <summary>
        /// 密钥
        /// </summary>
        private static readonly string secret = "ABC";
        /// <summary>
        /// 创建Token
        /// </summary>
        public static LoginResult CreteToken(T obj)
        {
            LoginResult rs = new LoginResult();
            JsonNetSerializer serializer = new JsonNetSerializer();
            JwtBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            JwtEncoder encoder = new JwtEncoder(new HMACSHA256Algorithm(), serializer, urlEncoder);
            var token = encoder.Encode(obj, secret);
            rs.Message = "创建token成功！";
            rs.Token = token;
            rs.Success = true;
            return rs;
        }
        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="token">客户端发过来的Token</param>
        public static T VlidateToken(string token)
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
            T json = decoder.DecodeToObject<T>(token, secret, verify: true);
            return json;
        }
    }
}