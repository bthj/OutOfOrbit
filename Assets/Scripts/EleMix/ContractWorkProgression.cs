using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContractWorkProgression : MonoBehaviour {

	// element source planets
	public GameObject fireSource, earthSource, waterSource, airSource;
	public TextMesh fireStockHUD, earthStockHUD, waterStockHUD, airStockHUD;
	// element rings / planet parts receiving elements
//	public GameObject fireReceptor, earthReceptor, waterReceptor, airReceptor;
//	public GameObject receptor1, receptor2, receptor3, receptor4;
	public List<GameObject> receptors;
	private Dictionary<Elements?, GameObject> elementReceptors;

	public ParticleSystem dustCloud;

	public LevelTutoralOverlay[] tutorialOverlays;
	public Texture2D tutorialOverlay;
	public Texture2D timeProgressNormal, timeProgressOvertime;
	public Texture2D fireProgressTex, earthProgressTex, waterProgressTex, airProgressTex;
	public Texture2D fireTex, earthTex, waterTex, airTex;
	public float progressBarBlinkSpeed;
	public float animationChangeTime = 1f;


	private HashIDs hash;

	// set to requirements from current contract, 
	// for easy reference and possibly efficiency, 
	// avoiding unneccecary multiple dict lookups (which could be a non-issue :)
	private float airRequiredUnits, earthRequiredUnits, fireRequiredUnits, waterRequiredUnits;

	// existing + required units
	private float airTotalUnits, earthTotalUnits, fireTotalUnits, waterTotalUnits;

	// element percentages of total element amount, for quick reference in progress bars
	private Dictionary<Elements, float> elementPercentages;
	// element progress percentage, updated on element hits, instead of on every update
	private Dictionary<Elements, float> elementProgressPercentages;

	private Dictionary<Elements, Texture2D> elementProgressTextures;

	private float timeLimit, timeLeft;
	private bool timerStarted;
	// because just setting timerStarted to false doesn't cut it, 
	// when depeding on having gone from the cameraAtRest state to not set timerSTarted to true again, 
	// after starting the exit animation, which takes some time to transition to.
	private bool donePlaying; 

	// how much we've gained of each element
	private float airTally, earthTally, fireTally, waterTally;

	// the amounts of elements we have to play with
	private float airStock, earthStock, fireStock, waterStock;
	// for usage ratio calculation:
	private float airStockInitial, earthStockInitial, fireStockInitial, waterStockInitial;

	private float initialDustCloudSize;

	private Elements? currentTargetElement;

	private GameObject finishedPlanet;

	private Vector2 timeProgressbarPosition, elementsProgressbarPosition;
	private Vector2 timeProgressbarSize, elementsProgressbarSize;

    private float elementProgressBarHeight;

    public string alertText;
    public Color alertColor;
    public Font alertFont;
    public int alertFontSize,maxAlertWidth;
    
	private GUIStyle alertStyle;	
	private SwipeToCreateElement swipeToCreateElement;

	private Animator cameraAnimator;

	void Awake() {
        MakeAlertStyle();
        maxAlertWidth = maxAlertWidth == 0 ? (int)(Screen.width * .72f) : maxAlertWidth;
        if(GameStatus.instance.CurrentContract == null)
        {
            GameStatus.instance.CurrentContract = MetaContract.FromContract(new Contract(1, 0, 0, 0, 0, 100));
		    GameStatus.instance.Inventory.AddElements(5,0,0,0);
			// GameStatus.instance.currentContract.AddExistingElements(1,1,1,1);

			// grab some texture for testing:
			GameStatus.instance.CurrentContract.finishedPlanetTexture = (Texture2D)fireSource.GetComponent<Renderer>().material.mainTexture;

			// hardcode a level into the current contract if we find one
			if( tutorialOverlays.Length > 0 ) {
				GameStatus.instance.CurrentContract.level = tutorialOverlays[0].level;
			}
        }


		// read the element requirements from the current contract:
		airRequiredUnits = GameStatus.instance.CurrentContract.requirements[Elements.AIR] - 
			GameStatus.instance.CurrentContract.results[Elements.AIR]; // .results hold whetever existing elements the planet has
		earthRequiredUnits = GameStatus.instance.CurrentContract.requirements[Elements.EARTH] -
			GameStatus.instance.CurrentContract.results[Elements.EARTH];
		fireRequiredUnits = GameStatus.instance.CurrentContract.requirements[Elements.FIRE] - 
			GameStatus.instance.CurrentContract.results[Elements.FIRE];
		waterRequiredUnits = GameStatus.instance.CurrentContract.requirements[Elements.WATER] -
			GameStatus.instance.CurrentContract.results[Elements.WATER];
		timeLimit = timeLeft = GameStatus.instance.CurrentContract.timeLimit;

		airTotalUnits = airRequiredUnits + GameStatus.instance.CurrentContract.results[Elements.AIR];
		earthTotalUnits = earthRequiredUnits + GameStatus.instance.CurrentContract.results[Elements.EARTH];
		fireTotalUnits = fireRequiredUnits + GameStatus.instance.CurrentContract.results[Elements.FIRE];
		waterTotalUnits = waterRequiredUnits + GameStatus.instance.CurrentContract.results[Elements.WATER];

		SetElementPercentages();
		SetElementProgressTexturesInADictionary();

		// set the tally to whatever is already existing on the planet:
		airTally = GameStatus.instance.CurrentContract.results[Elements.AIR];
		earthTally = GameStatus.instance.CurrentContract.results[Elements.EARTH];
		fireTally = GameStatus.instance.CurrentContract.results[Elements.FIRE];
		waterTally = GameStatus.instance.CurrentContract.results[Elements.WATER];

		elementProgressPercentages = new Dictionary<Elements, float>();
		elementProgressPercentages.Add( Elements.AIR, 0 );
		elementProgressPercentages.Add( Elements.EARTH, 0 );
		elementProgressPercentages.Add( Elements.FIRE, 0 );
		elementProgressPercentages.Add( Elements.WATER, 0 );
		
		// Put all available elements from the inventory to use.
		airStock = airStockInitial = GameStatus.instance.Inventory.CheckoutAllOfElement( Elements.AIR );
		earthStock = earthStockInitial = GameStatus.instance.Inventory.CheckoutAllOfElement( Elements.EARTH );
		fireStock = fireStockInitial = GameStatus.instance.Inventory.CheckoutAllOfElement( Elements.FIRE );
		waterStock = waterStockInitial = GameStatus.instance.Inventory.CheckoutAllOfElement( Elements.WATER );

		SetSourcePlanetVisibility();

		UpdateElementStockHUD();
	}


	void Start () {

		SetupElementReceptors();
	
		DeactivateAllElementReceptors();

		currentTargetElement = ElementsMethods.First();

		Debug.Log("airRequiredUnits: " + airRequiredUnits + ", earthRequiredUnits: " + earthRequiredUnits + ", fireRequiredUnits: " + fireRequiredUnits + ", waterRequiredUnits: " + waterRequiredUnits);

		if( ! IsElementRequired(currentTargetElement) ) {

			currentTargetElement = GetNextElementToWorkOn(currentTargetElement);
		}
		SetElementReceptorActivation( currentTargetElement, true );

		initialDustCloudSize = dustCloud.startSize;

		finishedPlanet = GameObject.Find("FinishedPlanet");
		// set the planet texture from current contract:
		finishedPlanet.GetComponent<Renderer>().material.SetTexture("_MainTex", GameStatus.instance.CurrentContract.finishedPlanetTexture);

		swipeToCreateElement = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<SwipeToCreateElement>();

		cameraAnimator = GameObject.Find("Main Camera").GetComponent<Animator>();
		// if the Animator isn't enabled, start the timer immediately:
		timerStarted = ! cameraAnimator.enabled;
		if( timerStarted ) swipeToCreateElement.ElementInstantiationEnabled = true;

		donePlaying = false;

		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

		timeProgressbarPosition = new Vector2( Screen.width * .14f, 0 );
		elementsProgressbarPosition = new Vector2( Screen.width * .14f, Screen.height * .95f );
		timeProgressbarSize = new Vector2( Screen.width * .72f , 14 );
		elementsProgressbarSize = new Vector2( Screen.width * .72f , 18 );

		elementProgressBarHeight = GetElementProgressBarHeight();

		UpdateElementProgressPercentages();
		UpdateDustCloudSize();
		SetPlanetCoverageToProgress();

		EnableLevelObstaclesAndStuff();

		SetTutorialOverlayToShow();
	}


	// Update is called once per frame
	void Update () {

		if( timerStarted  ) {

			UpdateTimeProgression();

		} else if( cameraAnimator.GetCurrentAnimatorStateInfo(0).IsName("cameraAtRest") && !donePlaying ) {
			// intro animation sequence done, time to start the timer
			timerStarted = true;
			cameraAnimator.speed = 1f;
			swipeToCreateElement.ElementInstantiationEnabled = true;
			EnableElementStockHUDs();
		}

		//finishedPlanet.renderer.material.SetFloat("_Cutoff", finishedPlanet.renderer.material.GetFloat("_Cutoff") - Time.deltaTime*0.3f );
	}

	/*
	 * TODO: Calling OnGUI is expensive, particularly bad on mobile devices.
	 * 			Unity seems to be working on optimizing this, like moving the implementation
	 * 			from C# to C++, but still there are said to be many draw calls (haven't tested)
	 * 		and so other alternatives should be explored.  
	 * 		Tried to render gui elements OnPostRender in a script attached to the camera - 
	 * 		see Scripts/Tests/OnPostRenderTest.cs - without success so far... 
	 */
	void OnGUI() {

		if( timerStarted ) {
            // RenderAlertText();
			RenderTimeProgression();
			RenderElementsProgression();
			RenderTutorialOverlay();
		}
	}


	public void SpeedUpAnimation() {

		StartCoroutine( GradualAnimationChange( cameraAnimator.speed, 5f ) );
	}
	IEnumerator GradualAnimationChange( float from, float to ) {
		float changeTimer = 0f;
		while( changeTimer < animationChangeTime && 
		      ! cameraAnimator.GetCurrentAnimatorStateInfo(0).IsName("cameraAtRest") ) {

			cameraAnimator.speed = Mathf.Lerp( from, to, changeTimer / animationChangeTime );
			changeTimer += Time.deltaTime;
			yield return null;
		}
	}


	// Called when elements are dragged to use from playarea's corners
	public void UseOneElementUnit( Elements type ) {

		tutorialOverlay = null;

		GameObject elementSource;
		float elementCoverage;
		// fade source planets' color
		switch( type ) {
		case Elements.EARTH:
			elementCoverage = --earthStock / earthStockInitial;
			elementSource = earthSource;
			break;
		case Elements.FIRE:
			elementCoverage = --fireStock / fireStockInitial;
			elementSource = fireSource;
			break;
		case Elements.WATER:
			elementCoverage = --waterStock / waterStockInitial;
			elementSource = waterSource;
			break;
		case Elements.AIR:
			elementCoverage = --airStock / airStockInitial;
			elementSource = airSource;
			break;
		default:
			elementSource = null;
			elementCoverage = 1f;
			break;
		}
		if( null != elementSource ) {

			SetCoverageBlend( elementSource, elementCoverage );

			UpdateElementStockHUD();

			if( ! HasElements( type ) ) {
				DecommissionElementSource( elementSource );

//				if( IsElementRequired( type ) ) {
//
//					QuitPlay();
//				}
			}
		}
	}


	public float GetElementRequirement( Elements type ) {
		float requirement;
		switch( type ) {
		case Elements.EARTH:
			requirement = earthRequiredUnits;
			break;
		case Elements.FIRE:
			requirement = fireRequiredUnits;
			break;
		case Elements.WATER:
			requirement = waterRequiredUnits;
			break;
		case Elements.AIR:
			requirement = airRequiredUnits;
			break;
		default:
			requirement = 0;
			break;
		}
		return requirement;
	}

	public float GetElementSourceVolume( Elements? type ) {
		float volume;
		switch( type ) {
		case Elements.EARTH:
			volume = earthStock;
			break;
		case Elements.FIRE:
			volume = fireStock;
			break;
		case Elements.WATER:
			volume = waterStock;
			break;
		case Elements.AIR:
			volume = airStock;
			break;
		default:
			volume = 0;
			break;
		}
		return volume;
	}
	public bool HasElements( Elements? type ) {

		return GetElementSourceVolume( type ) > 0;
	}


	// Called when elements reach the planet being worked on
	public void UpdateElementTally( Elements type, float deliveredAmount ) {

		switch (type){
		case Elements.EARTH:
			earthTally += deliveredAmount;
			break;
		case Elements.FIRE:
			fireTally += deliveredAmount;
			break;
		case Elements.WATER:
			waterTally += deliveredAmount;
			break;
		case Elements.AIR:
			airTally += deliveredAmount;
			break;
		}

		UpdateElementProgressPercentages();
		UpdateDustCloudSize();
		SetPlanetCoverageToProgress();

		if( ReachedElementGoal( type ) ) {

			DeactivateCurrentElementRingCollider( type );

			// TODO: register element surplus
            do
            {
                currentTargetElement = GetNextElementToWorkOn(type);
            } while (currentTargetElement != null && GetElementRequirement((Elements)currentTargetElement) == 0);
            
			if( null == currentTargetElement ) {
				// we've fulfilled all element goals, switch to the garage scene
				QuitPlay();
			} else {
				SetElementReceptorActivation( currentTargetElement, true );
			}
		}
	}

	public float GetElementProgressPercentage( Elements elementType ) {

		return elementProgressPercentages[ elementType ];
	}



	///// scene exit functions  /////
	
	public void ExitToGarage() {  // <- to be called from an animation event

		// stop asteroid spawning
		// now done in AsteroidSpawner.OnDestroy(): gameObject.GetComponent<AsteroidSpawner>().StopAllCoroutines();

		Application.LoadLevel(0);
	}

	public void QuitPlayIfElementTagCountIsBelowThreshold( string tag, int threshold ) {  // called from SwipeToCreateElement.ScaleObjectToDragDistance

		Elements? elementType = GetElementFromTag(tag);

		if( null != elementType && 
		   threshold > GameObject.FindGameObjectsWithTag( tag ).Length &&
		   ! HasElements(elementType) && 
		   IsElementRequired(elementType) &&
		   ! ReachedElementGoal(elementType) ) {
			
			QuitPlay();
		}
	}

	private Elements? GetElementFromTag( string tag ) {

		switch( tag ) {
		case Tags.elementAir:
			return Elements.AIR;
		case Tags.elementEarth:
			return Elements.EARTH;
		case Tags.elementFire:
			return Elements.FIRE;
		case Tags.elementWater:
			return Elements.WATER;
		default:
			return null;
		}
	}


	private void QuitPlay() {

		RegisterResultsInContract();

		PlayExitAnimation();

		timerStarted = false;  // freeze the time you've spent
		donePlaying = true;

		swipeToCreateElement.ElementInstantiationEnabled = false; // so any click speeds up the animation
	}

	private void RegisterResultsInContract() {

		GameStatus.instance.CurrentContract.SetResults( fireTally, earthTally, waterTally, airTally, timeLimit - timeLeft );
	}

	private void PlayExitAnimation() {

		cameraAnimator.SetBool( hash.timeIsUp, true );
		// Time's up, check if we've reached the contract's goal
		if( GetTotalProgressPercentage() >= 1f ) {
			// we've reached the contract's goal, do a grand exit
			cameraAnimator.SetBool( hash.completedContract, true );
		} else {
			// we didn't reach the goal, do a meagre exit
			cameraAnimator.SetBool( hash.completedContract, false );
		}
	}



	///// element info functions  /////

	private void SetElementPercentages() {
		
		float totalElementAmount = airRequiredUnits + earthRequiredUnits + fireRequiredUnits + waterRequiredUnits;

		elementPercentages = new Dictionary<Elements, float>();
		elementPercentages.Add(Elements.AIR, airRequiredUnits / totalElementAmount);
		elementPercentages.Add(Elements.EARTH, earthRequiredUnits / totalElementAmount);
		elementPercentages.Add(Elements.FIRE, fireRequiredUnits / totalElementAmount);
		elementPercentages.Add(Elements.WATER, waterRequiredUnits / totalElementAmount);
	}

	private void SetElementProgressTexturesInADictionary() {

		elementProgressTextures = new Dictionary<Elements, Texture2D>();
		elementProgressTextures.Add( Elements.AIR, airProgressTex );
		elementProgressTextures.Add( Elements.EARTH, earthProgressTex );
		elementProgressTextures.Add( Elements.FIRE, fireProgressTex );
		elementProgressTextures.Add( Elements.WATER, waterProgressTex );
	}

	private float GetElementProgressBarHeight() {

//		int totalElementsToWorkOn = 0;
//		if( airRequiredUnits > 0 ) totalElementsToWorkOn++;
//		if( earthRequiredUnits > 0 ) totalElementsToWorkOn++;
//		if( fireRequiredUnits > 0 ) totalElementsToWorkOn++;
//		if( waterRequiredUnits > 0 ) totalElementsToWorkOn++;
//
//		return elementsProgressbarSize.y / totalElementsToWorkOn;

		return elementsProgressbarSize.y;
	}

	private void UpdateElementProgressPercentages() {

		elementProgressPercentages[Elements.AIR] = airTally / airTotalUnits;
		elementProgressPercentages[Elements.EARTH] = earthTally / earthTotalUnits;
		elementProgressPercentages[Elements.FIRE] = fireTally / fireTotalUnits;
		elementProgressPercentages[Elements.WATER] = waterTally / waterTotalUnits;
	}

	private float GetTotalProgressPercentage() {

		float totalRequiredVolume = 
			fireRequiredUnits + earthRequiredUnits + waterRequiredUnits + airRequiredUnits;
		
		float totalGainedVolume = 
			GetGainNormalized(Elements.FIRE) + GetGainNormalized(Elements.EARTH) + 
				GetGainNormalized(Elements.WATER) + GetGainNormalized(Elements.AIR);

		float totalProgressPercentage = totalGainedVolume / totalRequiredVolume;
		Debug.Log( "Total progress: " + totalProgressPercentage );

		return totalProgressPercentage;
	}

	private float GetGainNormalized( Elements type ) {
		float gain;
		switch (type) {
		case Elements.EARTH:
			gain = earthTally > earthRequiredUnits ? earthRequiredUnits : earthTally;
			break;
		case Elements.FIRE:
			gain = fireTally > fireRequiredUnits ? fireRequiredUnits : fireTally;
			break;
		case Elements.WATER:
			gain = waterTally > waterRequiredUnits ? waterRequiredUnits : waterTally;
			break;
		case Elements.AIR:
			gain = airTally > airRequiredUnits ? airRequiredUnits : airTally;
			break;
		default:
			gain = 0;
			break;
		}
		return gain;
	}

	private Elements? GetNextElementToWorkOn( Elements? type ) {
		Elements? nextElement = type;
		// assumes type is not null when we begin:
		do {
			nextElement = nextElement.Next();

		} while( nextElement != null && ! IsElementRequired(nextElement) );
		return nextElement;
	}

	private bool IsElementRequired( Elements? type ) {
		bool required;
		switch (type){
		case Elements.EARTH:
			required = earthRequiredUnits > 0;
			break;
		case Elements.FIRE:
			required = fireRequiredUnits > 0;
			break;
		case Elements.WATER:
			required = waterRequiredUnits > 0;
			break;
		case Elements.AIR:
			required = airRequiredUnits > 0;
			break;
		default:
			required = false;
			break;
		}
		return required;
	}

	private bool ReachedElementGoal( Elements? type ) {
		bool goalReached = false;
		switch (type){
		case Elements.EARTH:
			Debug.Log( "earth tally: " + earthTally + ", req: " + earthRequiredUnits );
			goalReached = earthTally >= earthTotalUnits;
			break;
		case Elements.FIRE:
			goalReached = fireTally >= fireTotalUnits;
			break;
		case Elements.WATER:
			goalReached = waterTally >= waterTotalUnits;
			break;
		case Elements.AIR:
			goalReached = airTally >= airTotalUnits;
			break;
		}
		return goalReached;
	}

	private void SetElementReceptorActivation( Elements? type, bool active ) {

		elementReceptors[type].SetActive( active );
	}
	private void DeactivateAllElementReceptors() {
		foreach( Elements oneElement in System.Enum.GetValues(typeof(Elements)) ) {
			if( IsElementRequired(oneElement) ) {
				SetElementReceptorActivation( oneElement, false );
			}
		}
	}
	private void DeactivateCurrentElementRingCollider( Elements? type ) {

		elementReceptors[type].GetComponent<Collider>().enabled = false;
	}



	///// game object rendition /////


	private void SetSourcePlanetVisibility() {
		airSource.SetActive( airStockInitial > 0 );
		earthSource.SetActive( earthStockInitial > 0 );
		fireSource.SetActive( fireStockInitial > 0 );
		waterSource.SetActive( waterStockInitial > 0 );
	}

	private void UpdateElementStockHUD() {
		
		earthStockHUD.text = earthStock.ToString();
		fireStockHUD.text = fireStock.ToString();
		waterStockHUD.text = waterStock.ToString();
		airStockHUD.text = airStock.ToString();
	}
	
	private void EnableElementStockHUDs() {
		
		earthStockHUD.GetComponent<Renderer>().enabled = true;
		fireStockHUD.GetComponent<Renderer>().enabled = true;
		waterStockHUD.GetComponent<Renderer>().enabled = true;
		airStockHUD.GetComponent<Renderer>().enabled = true;
	}

	private void SetCoverageBlend( GameObject objectWithBlend2Textures, float blendFraction ) {
		
		// objectWithBlend2Textures.renderer.material.SetFloat("_Blend",  blendFraction );

		Debug.Log( "blendFraction: " + blendFraction );
		float cutoff = Mathf.SmoothStep( 0f, .7f, 1-blendFraction );
		objectWithBlend2Textures.GetComponent<Renderer>().material.SetFloat("_Cutoff", cutoff );
	}

	private void DecommissionElementSource( GameObject elementSource ) {

		// TODO: some fancy way of the empty element planet source going a way ...whirling, exploding, shrinking...

		Destroy( elementSource );
	}

	private void UpdateDustCloudSize() {
		
		float inversePercentCompleted = 1 - GetTotalProgressPercentage();
		if( inversePercentCompleted < 0f ) inversePercentCompleted = 0f;
		
		dustCloud.startSize = initialDustCloudSize * inversePercentCompleted;
	}

	private void SetPlanetCoverageToProgress() {

		// float cutoff = Mathfx.SinerpModifiedByBTHJ( 1f, 0.25f, GetTotalProgressPercentage() );
		float cutoff = Mathf.SmoothStep( 1f, .3f, GetTotalProgressPercentage() );

		finishedPlanet.GetComponent<Renderer>().material.SetFloat("_Cutoff", cutoff );
	}



	///// obstacles //////

	private void EnableLevelObstaclesAndStuff() {
		
		GameObject levesRoot = GameObject.Find("levels");
		foreach( Transform oneLevel in levesRoot.GetComponentsInChildren( typeof(Transform), true ) ) {
			
			if( oneLevel.name == GameStatus.instance.CurrentContract.level.name ) {
				
				oneLevel.gameObject.SetActive( true );
				break;
			}
		}
	}



	///// tutorial /////

	private void SetTutorialOverlayToShow() {
		tutorialOverlay = null;

		foreach( LevelTutoralOverlay oneOverlay in tutorialOverlays ) {

			if( oneOverlay.level.name == GameStatus.instance.CurrentContract.level.name ) {

				tutorialOverlay = oneOverlay.tutorialOverlay;
			}
		}
	}


	///// elements receptors initialisation /////

	private void SetElementReceptorTag( Elements elementType, GameObject receptor ) {

		switch (elementType){
		case Elements.EARTH:
			receptor.tag = Tags.elementEarth;
			break;
		case Elements.FIRE:
			receptor.tag = Tags.elementFire;
			break;
		case Elements.WATER:
			receptor.tag = Tags.elementWater;
			break;
		case Elements.AIR:
			receptor.tag = Tags.elementAir;
			break;
		}
	}

	private void SetElementReceptorTargetTexture( Elements elementType, GameObject receptor ) {
		switch (elementType){
		case Elements.EARTH:
			receptor.GetComponent<ElementRingReception>().SetTargetTexture( earthTex );
			break;
		case Elements.FIRE:
			receptor.GetComponent<ElementRingReception>().SetTargetTexture( fireTex );
			break;
		case Elements.WATER:
			receptor.GetComponent<ElementRingReception>().SetTargetTexture( waterTex );
			break;
		case Elements.AIR:
			receptor.GetComponent<ElementRingReception>().SetTargetTexture( airTex );
			break;
		}
	}

	private void SetupElementReceptors() {
		elementReceptors = new Dictionary<Elements?, GameObject>();
		int receptorIndex = 0;
		foreach( Elements oneElementType in GameStatus.instance.CurrentContract.Order ) {
			if( IsElementRequired(oneElementType) ) {
				
				receptors[receptorIndex].GetComponent<ElementRingReception>().element = oneElementType;

				SetElementReceptorTag( oneElementType, receptors[receptorIndex] );
				SetElementReceptorTargetTexture( oneElementType, receptors[receptorIndex] );
				
				elementReceptors.Add( oneElementType, receptors[receptorIndex] );
				
				receptorIndex++;
			}
		}
	}



	///// time progress bar /////

	private void UpdateTimeProgression() {
		
		// check if time is up
		timeLeft -= Time.deltaTime;
		if( timeLeft + timeLimit <= 0 ) { // we can go 1x overtime, thus the + timeLimit

			QuitPlay();
		}
	}
	
	private void RenderTimeProgression() {
		
		// TODO: update timer gui
		if( timeLeft < 0 ) {
			// we have gone overtime, render overtime gui
			float alphaLerp = Mathf.PingPong(Time.time, progressBarBlinkSpeed) / progressBarBlinkSpeed;
			float alpha = Mathf.Lerp( .25f, .5f, alphaLerp );
			GUI.color = new Color(Color.white.r, Color.white.g, Color.white.b, alpha);
			float barOriginOffset = timeProgressbarSize.x * Mathf.Clamp01( Mathf.Abs( timeLeft / timeLimit) );
			GUI.DrawTexture( new Rect(
				timeProgressbarPosition.x + barOriginOffset, 
				timeProgressbarPosition.y, 
				timeProgressbarSize.x - barOriginOffset, timeProgressbarSize.y), 
			                timeProgressOvertime );
		} else {
			// render normal gui time progress
			GUI.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0.25f);
			GUI.DrawTexture( new Rect(
				timeProgressbarPosition.x, timeProgressbarPosition.y, 
				timeProgressbarSize.x * Mathf.Clamp01(1 - timeLeft / timeLimit), timeProgressbarSize.y), 
			                timeProgressNormal );
		}
	}
	


	///// elements progress bar /////

	private void RenderElementsProgression() {
		
		// render the progress bar background

		float barXStartOffset = 0;
		int progressBarNo = 0;
		//GameStatus.instance.CurrentContract.Order[0]
		foreach( Elements oneElementType in GameStatus.instance.CurrentContract.Order ) {

			if( IsElementRequired(oneElementType) ) {
				DrawProgressBackgroundForOneElement( oneElementType, barXStartOffset );
				DrawProgressBarForOneElement( oneElementType, barXStartOffset, progressBarNo );
				barXStartOffset += elementsProgressbarSize.x * elementPercentages[oneElementType];
				//progressBarNo++;
			}
		}
	}

	private void DrawProgressBackgroundForOneElement( Elements elementType, float barStartOffset ) {
		float elementPercentage = elementPercentages[elementType];
		Texture2D barTexture = elementProgressTextures[elementType];

		GUI.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0.25f);

		GUI.DrawTexture( new Rect(
			elementsProgressbarPosition.x + barStartOffset, elementsProgressbarPosition.y, 
			elementsProgressbarSize.x * elementPercentage, elementsProgressbarSize.y), 
		                barTexture );
	}

	private void DrawProgressBarForOneElement( 
					Elements elementType, float barStartOffset, int progressBarNo ) {

		float elementPercentage = elementPercentages[elementType];
		float progressPercentage = elementProgressPercentages[elementType];
		float elementTotalSize = elementsProgressbarSize.x * elementPercentage;
		Texture2D barTexture = elementProgressTextures[elementType];

		GUI.color = new Color(Color.white.r, Color.white.g, Color.white.b, 1f);

		GUI.DrawTexture( new Rect(
			elementsProgressbarPosition.x + barStartOffset, elementsProgressbarPosition.y,  // + (progressBarNo * elementProgressBarHeight), 
			Mathf.Clamp(elementTotalSize * progressPercentage, 0, elementTotalSize), elementProgressBarHeight /*elementsProgressbarSize.y*/), 
		                barTexture );
	}



	///// render tutorial overlay /////

	private void RenderTutorialOverlay() {

		if( null != tutorialOverlay ) {

			GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height), tutorialOverlay );
		}
	}



    private void RenderAlertText()
    {
        GUIContent text = new GUIContent(alertText);
        float centerx = (Screen.width - Mathf.Min(alertStyle.CalcSize(text).x,maxAlertWidth)) * .5f;

        GUI.Label(new Rect(centerx, timeProgressbarSize.y+3, Mathf.Min(alertStyle.CalcSize(text).x,maxAlertWidth), alertStyle.CalcSize(text).y), alertText, alertStyle);
    }

    private void MakeAlertStyle()
    {
        GUIStyle alert = new GUIStyle();
        alert.fontSize = alertFontSize;
        alert.font = alertFont;
        alert.normal.textColor = alertColor;
        alert.wordWrap = true;
        alert.stretchHeight = true;
        alertStyle = alert;
    }



	[System.Serializable]
	public class LevelTutoralOverlay {

		public GameObject level;
		public Texture2D tutorialOverlay;
	}
}
