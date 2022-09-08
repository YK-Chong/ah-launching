using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour
{
    public Camera camera;
    public GameObject pivot;
    public float ratio;
    public bool onPress = false;
    public Vector2 initial;
    public Vector3 initialPivot;
    private Vector3 resetPivot;
    public ParticleSystem particle;
    private Vector3 target;
    private float _timer;
    private float _endScrollTime = 20f;
    public Color particleColor;
    private Color _hideParticleColor;
    private Material _particleMaterial;

    void Start()
    {
        Manager.Instance.OnStateChanged += OnStateChanged;
        resetPivot = initialPivot = pivot.transform.position;
        _particleMaterial = particle.GetComponent<ParticleSystemRenderer>().material;
        _hideParticleColor = new Color(particleColor.r, particleColor.g, particleColor.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Manager.Instance.CurrentState == Manager.State.Start)
        {
            _timer += Time.deltaTime;
            particle.gameObject.SetActive(true);
            if (Input.GetMouseButton(0))
            {
                if (onPress == false)
                {
                    initial = camera.ScreenToWorldPoint(Input.mousePosition);
                    initialPivot = pivot.transform.position;
                    onPress = true;
                }

                var t = camera.ScreenToWorldPoint(Input.mousePosition);
                var diff = initial.y - t.y;

                target = new Vector3(0, diff > 0 ? 0 : diff, 0);

                SetParticleColor(Color.Lerp(GetParticleColor(), particleColor, Time.deltaTime * 10f));
            }
            else
            {
                onPress = false;
                SetParticleColor(Color.Lerp(GetParticleColor(), _hideParticleColor, Time.deltaTime * 1f));
            }

            pivot.transform.position = Vector3.Lerp(pivot.transform.position, initialPivot + target, Time.deltaTime * ratio);
            if (pivot.transform.position.y < -50 || _timer >= _endScrollTime)
                Manager.Instance.ChangeState(Manager.State.ScrollEnd);
        }
    }

    private Color GetParticleColor()
    {
        return _particleMaterial.GetColor("_TintColor");
    }

    private void SetParticleColor(Color color)
    {
        _particleMaterial.SetColor("_TintColor", color);
    }

    private void OnStateChanged(Manager.State state)
    {
        Reset();
    }

    public void Reset()
    {
        particle.gameObject.SetActive(false);
        SetParticleColor(_hideParticleColor);
        initialPivot = resetPivot;
        target = initial = Vector3.zero;
        pivot.transform.position = resetPivot;
        _timer = 0;
    }
}
