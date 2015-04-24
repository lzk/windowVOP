using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Controls;

namespace VOP.Controls
{
    public class UserDefinedSizeValidationRule : ValidationRule
    {
        private double _minimumValue = 0;
        private double _maximumValue = 0;
        private int _decimalPlaces = 0;
        private string _errorMessage;

        public double MinimumValue
        {
            get { return _minimumValue; }
            set { _minimumValue = value; }
        }

        public double MaximumValue
        {
            get { return _maximumValue; }
            set { _maximumValue = value; }
        }

        public int DecimalPlaces
        {
            get { return _decimalPlaces; }
            set { _decimalPlaces = value; }
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

            double iNum = 0;
            if (double.TryParse(inputString, out iNum))
            {
                if (iNum < this.MinimumValue || iNum > this.MaximumValue)
                {
                    result = new ValidationResult(false, this.ErrorMessage);
                }

                var count = (from c in inputString
                             where c == '.'
                             select c).Count();

                if(count == 1)
                {
                    if((inputString.Length - inputString.IndexOf(".") - 1) > DecimalPlaces)
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                }else if(count > 1)
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
