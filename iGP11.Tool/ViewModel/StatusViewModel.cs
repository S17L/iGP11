﻿using iGP11.Tool.Common;
using iGP11.Tool.Model;

namespace iGP11.Tool.ViewModel
{
    public class StatusViewModel : ViewModel
    {
        private readonly ITaskRunner _runner;

        private bool? _state;
        private string _text;

        public StatusViewModel(ITaskRunner runner)
        {
            _runner = runner;
        }

        public bool? State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public void Set(StatusType status, string text)
        {
            _runner.Run(
                () =>
                {
                    State = status.Translate();
                    Text = text;
                });
        }
    }
}