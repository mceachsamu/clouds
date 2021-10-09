using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colliderController : MonoBehaviour
{
    ParticleSystem system;
    ParticleSystem.Particle[] m_Particles;

    public float minSize;

    public float maxSize;

    private Vector3 moveDirection;

    private float moveSpeed;

    private float emissionRate = 60.0f;

    private float spawnTransparency = 1.0f;

    public float transparencyFade;

    public Color baseColor;

    List<Vector4> customDat = new List<Vector4>();

    public void SetMaxSize(float max) {
        this.maxSize = max;
    }
    
    public void SetMinSize(float min) {
        this.minSize = min;
    }

    public void setMoveDirection(Vector3 moveDirection) {
        this.moveDirection = moveDirection;
    }

    public void setMoveSpeed(float moveSpeed) {
        this.moveSpeed = moveSpeed;
    }

    public void turnOn() {
        var emission = system.emission;
        emission.rate = emissionRate;
        this.spawnTransparency = 1.0f;
    }

    public void turnOff() {
        var emission = system.emission;
        emission.rate = emissionRate;
        emission.rate = 0.0f;
        this.spawnTransparency = 1.0f;
    }

    public bool isEmitting() {
        var emission = system.emission;
        return emission.rate.Evaluate(0.0f) != 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        system = this.GetComponent<ParticleSystem>();
        m_Particles = new ParticleSystem.Particle[system.main.maxParticles];
        system.GetComponent<ParticleSystemRenderer> ().renderingLayerMask = 0;

        // this.turnOff();
    }

    // Update is called once per frame
    void Update()
    {

        system.GetCustomParticleData(customDat, ParticleSystemCustomData.Custom2);
        
        int numParticlesAlive = system.GetParticles(m_Particles);
        for (int i = 0; i < numParticlesAlive; i++) {
            ParticleSystem.Particle particle = m_Particles[i];

            if (particle.startLifetime - 0.2f < particle.remainingLifetime) {
                float scale = Random.Range(minSize, maxSize);

                m_Particles[i].startSize = scale;
                this.handleTransparency(i);
            }

            m_Particles[i].position += this.moveDirection * this.moveSpeed;

        }

        system.SetParticles(m_Particles, numParticlesAlive);
        system.SetCustomParticleData(customDat, ParticleSystemCustomData.Custom2);
    }

    private void handleTransparency(int i) {
        customDat[i] = new Color(baseColor.r, baseColor.g, baseColor.b, spawnTransparency);

        if (this.isEmitting()) {
            spawnTransparency -= transparencyFade;
        }
        
        if (spawnTransparency <= 0.0f) {
            this.turnOff();
        }
    }
}
