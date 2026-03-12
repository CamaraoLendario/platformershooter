using Godot;
using System;
using System.Threading.Tasks;

public partial class PilotAttack : PilotController
{
	[Export] public PilotWeaponHolder weaponHolder;
	[Export] public MeleeAttack Melee;

	public override void _Ready()
	{
		base._Ready();

		Melee.dashed += () => 
		{
			isDashing = true;
			isJumping = false;
		};
		Melee.dashEnd += () =>
		{
			isDashing = false;
		};
	}

	public override void OnShootStart()
	{
		if (!IsAllowed() || Main.effectHandler.isFrozen) return;
		weaponHolder.Shoot(aimVector);
	}

	public void SetWeapon(Weapon weapon)
	{
		weaponHolder.CurrentWeapon = weapon;
	}

	public override void OnDropStart()
	{
		weaponHolder.DropWeapon();
	}

	public override void Start()
	{
		if (Main.IsDead) return;
		base.Start();
		weaponHolder.Show();
		if (sprite.Rotation > Mathf.Pi) sprite.Rotation -= Mathf.Pi * 2;
		if (sprite.Rotation < -Mathf.Pi) sprite.Rotation += Mathf.Pi * 2;

		Tween tween = CreateTween();
		tween.TweenProperty(sprite, "rotation", 0, 0.25f);
	}

	public override void OnMeleeStart()
	{
		if (!IsAllowed() || Main.effectHandler.isFrozen) return;
		Melee.Attack();
	}

	public override void End()
	{
		base.End();
		weaponHolder.Hide();
	}

	public override void Reset()
	{
		base.Reset();
		weaponHolder.DropWeapon();
	}
}
