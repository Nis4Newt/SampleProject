using System;

public abstract class ModelPanel<TModel> : BasePanel where TModel : class
{
    protected TModel Model { get; private set; }

    public void Bind(TModel model)
    {
        if (model == null) {
            throw new ArgumentNullException(nameof(model));
        }

        Model = model;
        if (model is INotifySource notifySource) {
            NotifyManager.Register(notifySource);
        }

        OnBound();
    }

    protected virtual void OnDestroy()
    {
        if (Model == null) {
            return;
        }

        OnUnbound();

        if (Model is INotifySource notifySource) {
            NotifyManager.Unregister(notifySource);
        }
    }

    protected abstract void OnBound();

    protected virtual void OnUnbound() { }
}
