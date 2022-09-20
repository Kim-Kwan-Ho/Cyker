using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(GetDamage_Exploder))] //Explosion������Ʈ�� �߰��� �� GetDamage_Exploder�� ���ٸ� �ڵ����� �߰����ش�.
public class Explosion : MonoBehaviour
{
	public GameObject explosionPrefab; //���� ������
	public float radius = 5;
	public float power = 1;
	public float upPower = 0.5f;
	public float duration = 1f;
	public float damage = 1;

	[Range(0.1f, 60)]
	public float delayTime = 1f;
	public bool drawExplsionRange = true;


	[HideInInspector]
	public bool exploded = false; // �����ߴ��� üũ
	private bool explode_Dur = false; // ���������� üũ



	public IEnumerator GenerateExplosion(float duration, float scale) // ���� ����Ʈ ����
	{
		GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
		explosion.transform.localScale *= scale;
		yield return new WaitForSeconds(duration);
		explode_Dur = false;
		Destroy(explosion);
	}

	private void ExplosionAddForce(float radius, float explosionDamage) // ���� ���� �ִ� ��ü�� �������� �ְ� ���������κ��� �о��.
	{
		foreach (Collider col in Physics.OverlapSphere(transform.position, radius))
		{
			if (col.GetComponent<Rigidbody>() != null && !col.GetComponent<Rigidbody>().isKinematic) // �������� �޴��� üũ
			{
				float obj_distance = Vector3.Distance(col.transform.position, transform.position);
				float proportionalDistance = Mathf.Clamp(obj_distance * 1.5f / radius, 0.5f, 1); //�Ÿ����
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
		if (!exploded) // ������ ���� ���� ���¶��
		{
			if (explode_Dur) // �����ϴ� ���̸�
			{
				exploded = true;
				StartCoroutine(GenerateExplosion(duration, radius / 7));
				ExplosionAddForce(radius, damage);
			}
		}
		else // ���� �ߴٸ� ������ ���� �̵��� ����
		{
			gameObject.GetComponent<MeshRenderer>().enabled = false; // MeshRenderer�� ��
			gameObject.GetComponent<Collider>().enabled = false; // BoxCollider�� ��
			gameObject.GetComponent<Rigidbody>().useGravity = false; //�߷��� ��
			gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}
	private void OnDrawGizmos() // ���� ���� ǥ��
	{
		Gizmos.color = Color.red;
        if (drawExplsionRange)
        {
			Gizmos.DrawWireSphere(gameObject.transform.position, radius);
		}
	}

	public void Set_Explode_Dur(bool explode_Dur) { this.explode_Dur = explode_Dur; }
}
