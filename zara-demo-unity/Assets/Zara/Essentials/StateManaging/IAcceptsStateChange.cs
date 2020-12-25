namespace ZaraEngine.StateManaging {

    public interface IAcceptsStateChange {

        IStateSnippet GetState();

        void RestoreState(IStateSnippet savedState);

    }

}