﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace System.Hooks
{

    public enum SYSMSGFILTER_Messages:int
    {
        MSGF_DIALOGBOX = 0,
        MSGF_MESSAGEBOX    = 1,
        MSGF_MENU          = 2,
        MSGF_SCROLLBAR    =  5,
        MSGF_NEXTWINDOW    = 6,
        MSGF_MAX          =  8 ,                      
        MSGF_USER   =        4096
    }

    /// <summary>
    /// The system calls this function after an input event occurs in a dialog box, message box, menu, or scroll bar, but before the message generated by the input event is processed.
    /// </summary>
    public class WH_SYSMSGFILTER : IHook
    {

        public int Code { get; private set; }
        public IntPtr wParam { get; private set; }
        public IntPtr lParam { get; private set; }
        public Process Caller { get; private set; }
        public DateTime Time { get; private set; }


        /// <summary>
        /// The system calls this function after an input event occurs in a dialog box, message box, menu, or scroll bar, but before the message generated by the input event is processed.
        /// </summary>
        new public const string Description = "The system calls this function after an input event occurs in a dialog box, message box, menu, or scroll bar, but before the message generated by the input event is processed.";

        /// <summary>
        /// Specifies whether the hook procedure can be intercepted
        /// </summary>
        public override bool InterceptEffective
        {
            get { return false; }
        }

        public SYSMSGFILTER_Messages Message
        {
            get
            {
                if (Code > (int)SYSMSGFILTER_Messages.MSGF_MAX) { return SYSMSGFILTER_Messages.MSGF_USER; }
                return (SYSMSGFILTER_Messages)Code;
            }
            private set { }
        }



        public MSG Attachment
        {
            get
            {
                return MarshalHelper.GetStructFromProcess<MSG>(Caller, lParam);
            }
            private set { }
        }


        public override string ToString()
        {
            return "User pressed a " + Message + " at X=" + Attachment.pt.X + ",Y=" + Attachment.pt.Y + " it will get: " + (WindowsMessages)Attachment.message;
        }



        /// <summary>
        /// Translates Winows message into usable format and extracts all information
        /// </summary>
        public WH_SYSMSGFILTER(HookArguments Msg) : base(Msg)
        {
            if (Msg == null) { return; }

            this.Code = Msg.nCode;
            this.wParam = Msg.wParam;
            this.lParam = Msg.lParam;
            this.Caller = Msg.Process;
            this.Time = Msg.TimeStamp;
        }
    }
}
