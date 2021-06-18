using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zcy.Services.Welcome;

namespace Zcy.ViewModels.Welcome
{
    public class IntroduceViewModel : BaseViewModel, IIntroduce
    {
        public IntroduceViewModel(IEventAggregator events) : base(events)
        {
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            return base.OnActivateAsync(cancellationToken);
        }

        public void ToLink(object data)
        {

            //System.Diagnostics.Process.Start("http://baidu.com");

        }




    }
}
