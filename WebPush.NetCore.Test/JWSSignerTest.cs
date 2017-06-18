﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Crypto.Parameters;
using WebPush.Util;

namespace WebPush.Test
{
    [TestClass]
    public class JWSSignerTest
    {
        [TestMethod]
        public void TestGenerateSignature()
        {
            ECPrivateKeyParameters privateKey = ECKeyHelper.GetPrivateKey(new byte[32]);

            Dictionary<string, object> header = new Dictionary<string, object>();
            header.Add("typ", "JWT");
            header.Add("alg", "ES256");

            Dictionary<string, object> jwtPayload = new Dictionary<string, object>();
            jwtPayload.Add("aud", "aud");
            jwtPayload.Add("exp", 1);
            jwtPayload.Add("sub", "subject");

            JWSSigner signer = new JWSSigner(privateKey);
            string token = signer.GenerateSignature(header, jwtPayload);

            string[] tokenParts = token.Split('.');

            Assert.AreEqual(3, tokenParts.Length);

            string encodedHeader = tokenParts[0];
            string encodedPayload = tokenParts[1];
            string signature = tokenParts[2];

            string decodedHeader = Encoding.UTF8.GetString(UrlBase64.Decode(encodedHeader));
            string decodedPayload = Encoding.UTF8.GetString(UrlBase64.Decode(encodedPayload));

            Assert.AreEqual(@"{""typ"":""JWT"",""alg"":""ES256""}", decodedHeader);
            Assert.AreEqual(@"{""aud"":""aud"",""exp"":1,""sub"":""subject""}", decodedPayload);

            byte[] decodedSignature = UrlBase64.Decode(signature);
            int decodedSignatureLength = decodedSignature.Length;


            bool isSignatureLengthValid = decodedSignatureLength == 66 || decodedSignatureLength == 64;
            Assert.AreEqual(true, isSignatureLengthValid);
        }
    }
}
