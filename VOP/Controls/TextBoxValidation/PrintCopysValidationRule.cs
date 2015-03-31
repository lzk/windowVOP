using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Controls;

namespace VOP.Controls
{
    public class PrintCopysValidationRule : ValidationRule
    {
        private int _minimumValue = 1;
        private int _maximumValue = 99;
        private string _errorMessage;

        public int MinimumValue
        {
            get { return _minimumValue; }
            set { _minimumValue = value; }
        }

        public int MaximumValue
        {
            get { return _maximumValue; }
            set { _maximumValue = value; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public override ValidationResult Validate(object value,
            CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            string inputString = (value ?? string.Empty).ToString();

            int iNum = 0;
            if(Int32.TryParse(inputString, out iNum))
            {
                if (iNum < this.MinimumValue || iNum > this.MaximumValue)
                {
                    result = new ValidationResult(false, this.ErrorMessage);
                }
            }
            else
            {
                result = new ValidationResult(false, this.ErrorMessage);
            }
          
            return result;
        }
    }
}
