using System;
using System.Collections.Generic;

namespace Exam_Helper
{
    public partial class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        public string PackSet { get; set; }
        public string QuestionSet { get; set; }
        public string Img { get; set; }
        public string IgnorePacks { get; set; }
        public string IgnoreQues { get; set; }
    }
}
