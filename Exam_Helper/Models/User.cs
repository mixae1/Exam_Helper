using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Exam_Helper
{
    public partial class User:IdentityUser
    {
        public string Login { get; set; }
        public string PackSet { get; set; }
        public string QuestionSet { get; set; }
        public string Img { get; set; }
        public string IgnorePacks { get; set; }
        public string IgnoreQues { get; set; }
    }
}
