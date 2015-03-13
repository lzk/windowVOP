using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace VOP.Controls
{
    public enum enumIdCardType
    {
        HouseholdRegister,
        IdCard,
        MarriageCertificate,
        Passport,
        RealEstateEvaluator,
        DriverLicense,
        Diploma,
        StudentIDcard,
        BirthCertificate,
        BankCards
    }

    public enum enumIdCardPrintSides
    {
        OneSide,
        TwoSides
    }

    public class IdCardTypeItem : DependencyObject
    {
        public static readonly DependencyProperty TypeIdProperty =
         DependencyProperty.Register("TypeId", typeof(enumIdCardType), typeof(IdCardTypeItem));

        public static readonly DependencyProperty NameProperty =
         DependencyProperty.Register("Name", typeof(string), typeof(IdCardTypeItem));

        public static readonly DependencyProperty PrintSidesProperty =
         DependencyProperty.Register("PrintSides", typeof(enumIdCardPrintSides), typeof(IdCardTypeItem));

        public static readonly DependencyProperty WidthProperty =
         DependencyProperty.Register("Width", typeof(double), typeof(IdCardTypeItem));

        public static readonly DependencyProperty HeightProperty =
         DependencyProperty.Register("Height", typeof(double), typeof(IdCardTypeItem));

        public enumIdCardType TypeId
        {
            get { return (enumIdCardType)GetValue(TypeIdProperty); }
            set { SetValue(TypeIdProperty, value); }
        }

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public enumIdCardPrintSides PrintSides
        {
            get { return (enumIdCardPrintSides)GetValue(PrintSidesProperty); }
            set { SetValue(PrintSidesProperty, value); }
        }

        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }
    }
}
