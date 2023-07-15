// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC.Areas.Identity.Models.Account
{
    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; } = default!;

        public ICollection<SelectListItem> Providers { get; set; } = default!;

        public string ReturnUrl { get; set; } = default!;

        public bool RememberMe { get; set; } = default!;
    }
}
