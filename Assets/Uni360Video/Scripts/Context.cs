using UnityEngine;

public class Context : MonoBehaviour {
    [SerializeField]
    Uni360Video.Options options;

    public void Show360Video () {
        Uni360Video.Init(options);
    }

    public void Stop360Video() {
        Uni360Video.Stop();
    }
}
