using System.Collections.Generic;
using System;



// Can send messages
public interface pDelegateSender
{
    public List<WeakReference<pDelegateReceiver>> mDelegates { get; }

    public void CallAction(object[] args)
    {
        foreach (var del in mDelegates)
        {
            pDelegateReceiver receiver;
            del.TryGetTarget( out receiver );

            if( del == null )
            {
                mDelegates.Remove( del );
            }

            receiver?.Action(this, args);
        }
    }

    public void AddDelegate(pDelegateReceiver del)
    {
        mDelegates.Add(new WeakReference<pDelegateReceiver>( del ));
    }


    public void RemoveDelegate(pDelegateReceiver del)
    {
        int i = 0;
        foreach( var weak in mDelegates)
        {
            pDelegateReceiver receiver;
            weak.TryGetTarget(out receiver);

            if( receiver == del )
            {
                mDelegates.RemoveAt( i );
                return;
            }

            ++i;
        }
    }
}




// Can receive messages
public interface pDelegateReceiver
{
    public void Action( pDelegateSender sender, object[] args );
}
