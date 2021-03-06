﻿using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Epicoin.Library.Tools
{
    public static class Hash
    {
        public static string CpuGenerate(string data)
        {
            var hash = (new SHA256Managed()).ComputeHash(Encoding.UTF8.GetBytes(data));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }

        public static string GpuGenerate(string data)
        {
            throw new NotImplementedException("Hash.GpuGenerate: TO DO");
        }
    }
}