using System.Collections.Generic;
using System.Linq;

using iGP11.Tool.Common;
using iGP11.Tool.Model;
using iGP11.Tool.Shared.Plugin;

namespace iGP11.Tool.ViewModel
{
    public class PickEffectTypeViewModel : ViewModel
    {
        public PickEffectTypeViewModel(IEnumerable<EffectType> effectTypes)
        {
            EffectTypes = effectTypes
                .Select(effect => new ValueDescription(effect.GetComponentLocalizableName(), effect))
                .ToArray();

            CancelCommand = new ActionCommand(Close, () => true);
            SubmitCommand = new ActionCommand(Submit, () => true);
        }

        public delegate void CancelEventHandler();

        public delegate void SubmitEventHandler(EffectType effect);

        public event CancelEventHandler OnCancelled;

        public event SubmitEventHandler OnSubmitted;

        public IActionCommand CancelCommand { get; }

        public EffectType EffectType { get; set; }

        public IEnumerable<ValueDescription> EffectTypes { get; }

        public IActionCommand SubmitCommand { get; }

        private void Close()
        {
            OnCancelled?.Invoke();
        }

        private void Submit()
        {
            OnSubmitted?.Invoke(EffectType);
        }
    }
}