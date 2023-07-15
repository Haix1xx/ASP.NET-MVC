﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Areas.Identity.Models.Manage
{
    public class RemoveLoginViewModel
    {
        public string LoginProvider { get; set; } = default!;
        public string ProviderKey { get; set; } = default!;
    }
}