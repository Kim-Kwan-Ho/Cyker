using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(GetDamage_Exploder))] //Explosion컴포넌트를 추가할 때 GetDamage_Exploder가 없다면 자동으로 추가해준다.
public class Explosion : MonoBehaviour
{
	public GameObject explosionPrefab; //폭발 프리펩
	public float radius = 5;
	public float power = 1;
	public float upPower = 0.5f;
	public float duration = 1f;
	public float damage = 1;

	[Range(0.1f, 60)]
	public float delayTime = 1f;
	public bool drawExplsionRange = true;


	[HideInInspector]
	public bool exploded = false; // 폭발했는지 체크
	private bool explode_Dur = false; // 폭발중인지 체크



	public IEnumerator GenerateExplosion(float duration, float scale) // 폭발 이펙트 생성
	{
		GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
		explosion.transform.localScale *= scale;
		yield return new WaitForSeconds(duration);
		explode_Dur = false;
		Destroy(explosion);
	}

	private void ExplosionAddForce(float radius, float explosionDamage) // 범위 내에 있는 물체에 데미지를 주고 폭발점으로부터 밀어낸다.
	{
		foreach (Collider col in Physics.OverlapSphere(transform.position, radius))
		{
			if (col.GetComponent<Rigidbody>() != null && !col.GetComponent<Rigidbody>().isKinematic) // 물리력을 받는지 체크
			{
				float obj_distance = Vector3.Distance(col.transform.position, transform.position);
				float proportionalDistance = Mathf.Clamp(obj_distance * 1.5f / radius, 0.5f, 1); //거리비례
				col.GetComponent<Rigidbody>().AddExplosionForce(power * proportionalDistance, transform.position, radius, upPower, ForceMode.Impulse);
                if (col.GetComponent<EnemyMain>())
                {
					col.GetComponent<EnemyMain>().TakeDamage(explosionDamage * proportionalDistance);
				}
			}
		}
	}

    void Start()
	{
		if(delayTime <= 0)
        {
			delayTime = 0.1f;
		}
	}

    private void FixedUpdate()
    {
		if (!exploded) // 폭발을 하지 않은 상태라면
		{
			if (explode_Dur) // 폭발하는 중이면
			{
				exploded = true;
				StartCoroutine(GenerateExplosion(duration, radius / 7));
				ExplosionAddForce(radius, damage);
			}
		}
		else // 폭발 했다면 랜더를 끄고 이동을 멈춤
		{
			gameObject.GetComponent<MeshRenderer>().enabled = false; // MeshRenderer를 끔
			gameObject.GetComponent<Collider>().enabled = false; // BoxCollider를 끔
			gameObject.GetComponent<Rigidbody>().useGravity = false; //중력을 끔
			gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}
	private void OnDrawGizmos() // 폭발 범위 표시
	{
		Gizmos.color = Color.red;
        if (drawExplsionRange)
        {
			Gizmos.DrawWireSphere(gameObject.transform.position, radius);
		}
	}

	public void Set_Explode_Dur(bool explode_Dur) { this.explode_Dur = explode_Dur; }
}
