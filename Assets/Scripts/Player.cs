using Godot;
using System;

public partial class Player : CharacterBody2D
{
//region Player State, Properties, Animations, Audio, and Signals
	//audios del jugador 
	private AudioStreamPlayer audioJump;
	private AudioStreamPlayer audioHit;
	private AudioStreamPlayer audioDeath;
	
	//Para obtener la animacion del corazon
	private Node2D heartBar;
	private AnimatedSprite2D heart;
	//Booleano para cuando es golepado
	private bool isHit = false;
		//Booleano que le permite saltar o no 
	public bool canJump { get; set; } = true;
	[Export]
	public int GRAVITY { get; private set; } = 4200;
	
	[Export]
	public int JUMP_SPEED { get; private set; } = 1800;

	private AnimatedSprite2D anim;
	private AnimationPlayer animPlayer;
	private String name;
	public int hp;
	private int hpInicial;
	public int armor;
	public bool isDead = false;
	//Declaracion  de señal para la muerte del jugador
	[Signal]
	public delegate void PlayerDiedEventHandler();
//endregion
	
	//Funcion para hacer las animaciones de la vida del jugador 
	public void UpdateHeart()
	{
		if (heart == null)
		{
			GD.PrintErr("Heart no está asignado.");
			return;
		}
		if(isDead)
		{
			return;
		}

		string animName = "";
		int medidor = hpInicial/3;

		if (hp >= hpInicial-13)
			animName = "Full";
		else if (hp >= (medidor + medidor))
			animName = "Lose4";
		else if (hp >= medidor)
			animName = "Lose6";
		//else
		//	animName = $"Lose{6 - hp}";

		if (heart.SpriteFrames.HasAnimation(animName))
		{
			heart.Play(animName);
		}
		else
		{
			GD.PrintErr($"La animación '{animName}' no existe en el AnimatedSprite2D.");
		}
	}
	
	public override void _Ready()
	{
		// Obtener el nodo padre y hacerle cast a tipo Main
		var mainNode = GetTree().Root.GetNodeOrNull<Main>("Main");
		if (mainNode is Main main)
		{
			Jugador datos = main.setDatosPersonaje();

			// Asignar los datos del jugador
			name = datos.name;
			hp = datos.hp;
			hpInicial = datos.hp;
			armor = datos.armor;

			GD.Print($"Jugador cargado desde Main: {name}, HP: {hp}, Armor: {armor}");
		}
		else
		{
			GD.PrintErr("No se pudo obtener el nodo Main desde Player.");
		}
		try {
			audioJump = GetNode<AudioStreamPlayer>("AudioJump");
			audioHit = GetNode<AudioStreamPlayer>("AudioHit");
			audioDeath = GetNode<AudioStreamPlayer>("AudioDeath");
			anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			
			try {
				var onGame = GetNodeOrNull<CanvasLayer>("../OnGame");
				if (onGame == null) {
					GD.PrintErr("No se encontró el nodo 'OnGame'");
					return;
				}

				heartBar = onGame.GetNodeOrNull<Node2D>("HeartBar");
				if (heartBar == null) {
					GD.PrintErr("No se encontró el nodo 'HeartBar' dentro de 'OnGame'");
					return;
				}

				heart = heartBar.GetNodeOrNull<AnimatedSprite2D>("Heart");
				if (heart == null) {
					GD.PrintErr("No se encontró el nodo 'Heart' dentro de 'HeartBar'");
					return;
				}
			} catch (Exception e) {
				GD.PrintErr("Error accediendo a nodos: ", e.Message);
			}
		} catch (Exception e) {
			GD.PrintErr("Error accediendo a nodos: ", e.Message);
		}
		
		UpdateHeart();
		if (Global.SelectedCharacter == "res://Assets/Prefabs/Knigth.tscn")
		{
			armor = 6;
			GD.Print("Caballero detectado. Armadura inicial: " + armor);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		//GD.Print($"Estado isHit: {isHit} | Animación: {anim.Animation} | Colisión: {GetNode<CollisionShape2D>("CollisionShape2D").Disabled}");
		if (!isDead && isHit) return; // si está golpeado pero no muerto, no hacer animaciones normales
		//Si está muerto, solo aplicar gravedad
		if (isDead)
		{
			Velocity = new Vector2(0, Velocity.Y + (float)(GRAVITY * delta)); 
			MoveAndSlide();
			return;
		}
		Velocity = new Vector2(Velocity.X, Velocity.Y + (float)(GRAVITY * delta));
		
		//region Animations
		if (IsOnFloor())
		{
			if(Velocity.Y>0f)
			{
			if (Input.IsActionJustPressed("jump_key") && canJump)
			{
				Velocity = new Vector2(Velocity.X, -JUMP_SPEED);
				audioJump.Play();
			}
			else
			{
				try{
					anim.Play("Moving");
				}catch(NullReferenceException x){
				}
			}
			}
			else
			{
				try{
					anim.Play("Idle");
				}catch(NullReferenceException x){
				}
				
			}
		}
		else
		{
			try{
			anim.Play("Jump");}catch(NullReferenceException x){	
			}
		}
//endregion

		MoveAndSlide();
	}

	public async void Hit()
	{
	if (isHit) return;
	isHit = true;
	try
	{
		// Detener y reproducir animación (usando variable existente)
		anim.Stop();
		anim.Play("Hit");
		audioHit.Play();

		// Calcular duración exacta (frame count * tiempo por frame)
		double hitDuration = (anim.SpriteFrames.GetFrameCount("Hit") / anim.SpriteFrames.GetAnimationSpeed("Hit"))/anim.SpeedScale;
		
		// Esperar animación + margen de seguridad
		await ToSignal(GetTree().CreateTimer(hitDuration), "timeout");

		// Reset seguro (evita que quede en estado intermedio)
		if (anim.IsPlaying() && anim.Animation == "Hit")
		{
			anim.Play("Moving");
		}
	}
	finally
	{
		isHit = false;
	}
		
		GD.Print("Jugador golpeado");
	if (armor > 0){
		armor -= 1;
		GD.Print("Armor restante: " + armor);
	}else{
		hp -= 1;
		if (heart != null) UpdateHeart();
		GD.Print("HP restante: " + hp);
	}
	
	if (hp <= 0) {
			Die();
	}
	
	var mainNode = GetParent() as Main;
	mainNode.actualizarPlayer(hp, armor);
}

	private void Die()
	{
		isDead = true;

		// Detener movimiento
		GRAVITY = 0;
		SetProcess(false);
		SetPhysicsProcess(false);
		GD.Print("El jugador ha muerto");
		audioDeath.Play();
		anim.Play("Death");
		// Esperar a que termine la animación para reiniciar
		EmitSignal(SignalName.PlayerDied);
	}
}
