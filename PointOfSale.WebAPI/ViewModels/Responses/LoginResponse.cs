﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.WebAPI.ViewModels.Responses
{
    public class LoginResponse
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
