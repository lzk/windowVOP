using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace VOP.Controls
{
/// <summary>
/// usage sample 
/// 
/// 1.  
///                 MessageBoxExResult result = MessageBoxEx.Show(MessageBoxExStyle.YesNo, this, "content...  ", "caption");
///                 if (MessageBoxExResult.Yes == result)
///                  {
///                      ;
///                  }
///                 else if (MessageBoxExResult.No == result)
///                 {
///                     ;
///                 }
///
///
///   
///   2.            MessageBoxExResult result = MessageBoxEx.Show(MessageBoxExStyle.YesNo, this, "content...  ", "caption");
/// 
/// 
/// </summary>

    public enum MessageBoxExStyle
    {
        Simple,
        YesNo
    }


    // Summary:
    //     Specifies which message box button that a user clicks. MessageBoxExResult
    //     is returned by the MessageBoxEx.Show method.
    public enum MessageBoxExResult
    {
        // Summary:
        //     The message box returns no result.
        None = 0,
        //
        // Summary:
        //     The result value of the message box is OK.
        OK = 1,
        //
        // Summary:
        //     The result value of the message box is Cancel.
        Cancel = 2,
        //
        // Summary:
        //     The result value of the message box is Yes.
        Yes = 6,
        //
        // Summary:
        //     The result value of the message box is No.
        No = 7,
    }



    class MessageBoxEx
    {
        public static MessageBoxExResult Show(MessageBoxExStyle style, Window owner=null, string messageBoxText = "", string caption = "", Uri uri = null)
        {
            Window msg = null;

            if (MessageBoxExStyle.Simple == style)
            {
                msg = new MessageBoxEx_Simple(messageBoxText, caption);
            }
            else if (MessageBoxExStyle.YesNo == style)
            {

                msg = new MessageBoxEx_YesNo(messageBoxText, caption);
            }

            if (null != owner)
            {
               try
               {
                   msg.Owner = owner;
               }
                catch
               {
               }               
            }

            msg.ShowDialog();

            if (MessageBoxExStyle.YesNo == style)
            {
                MessageBoxEx_YesNo msg_YesNo = msg as MessageBoxEx_YesNo;
                if (null != msg_YesNo)
                {
                    return msg_YesNo.messageBoxExResult;
                }
            }

            return MessageBoxExResult.None;
        }

        public static MessageBoxExResult Show(MessageBoxExStyle style, string messageBoxText = "", string caption = "", Uri uri = null)
        {
            return Show(style, null, messageBoxText, caption, uri);           
        }
    }
}
