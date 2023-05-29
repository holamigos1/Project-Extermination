// Designed by Kinemation, 2023

using System;
using System.Collections.Generic;
using Plugins.Kinemation.FPSFramework.Runtime.Core.Data;
using Plugins.Kinemation.FPSFramework.Runtime.Core.States;
using UnityEngine;
using Weapons.Data;
using Random = UnityEngine.Random;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core
{
	public class RecoilAnimation : MonoBehaviour
	{
		public Vector3 OutRotation { get; private set; }
		public Vector3 OutLocation { get; private set; }
		
		public  bool isAiming;
		public  FireMode fireMode;
		
		private StartRest       _canRestLoc;
		private StartRest       _canRestRot;
		private bool            _isSmoothingEnabled;
		private bool            _isLooping;
		private bool            _isPlaying;
		private float           _fireRate;
		private float           _lastFrameTime;
		private float           _lastTimeShot;
		private Vector2         _noiseOut;
		private Vector2         _noiseTarget;
		private float           _playBack;
		private float           _pushOut;
		private float           _pushTarget;
		private Vector3         _rawLocationOut;
		private Vector3         _rawRotationOut;
		private RecoilAnimData  _recoilData;
		private Vector3         _smoothLocationOut;
		private Vector3         _smoothRotationOut;
		private Vector3         _startValueLocation;
		private Vector3         _startValueRotation;
		private int             _stateIndex;
		private List<AnimState> _animationStateMachine;
		private Vector3         _targetLocation;
		private Vector3         _targetRotation;
		private Vector3DCurve    _tempLocationCurve;
		private Vector3DCurve    _tempRotationCurve;
		
		private float Delta =>
			Time.unscaledTime - _lastTimeShot;

		private void Update()
		{
			if (_isPlaying)
			{
				UpdateSolver();
				UpdateTimeline();
			}

			ApplySmoothing();

			Vector3 finalLoc = _smoothLocationOut;

			ApplyNoise(ref finalLoc);
			ApplyPushback(ref finalLoc);

			OutRotation = _smoothRotationOut;
			OutLocation = finalLoc;
		}

		public void Init(RecoilAnimData data, float fireRate, FireMode newFireMode)
		{
			_pushTarget = 0f;
			_recoilData = data;
			_fireRate = fireRate;
			fireMode = newFireMode;
			OutRotation = Vector3.zero;
			OutLocation = Vector3.zero;
			_targetRotation = Vector3.zero;
			_targetLocation = Vector3.zero;
			_noiseTarget = Vector2.zero;

			SetupStateMachine();
		}

		public void Play()
		{
			//Iterate through each transition, if true execute
			for (var i = 0; i < _animationStateMachine.Count; i++)
				if (_animationStateMachine[i].CheckCondition.Invoke())
				{
					_stateIndex = i;
					break;
				}

			_animationStateMachine[_stateIndex].OnPlay.Invoke();
			_lastTimeShot = Time.unscaledTime;
		}

		public void Stop()
		{
			_animationStateMachine[_stateIndex].OnStop.Invoke();
			_isLooping = false;
		}

		private void CalculateTargetData()
		{
			float pitch = Random.Range(_recoilData.pitch.x, _recoilData.pitch.y);
			float yawMin = Random.Range(_recoilData.yaw.x, _recoilData.yaw.y);
			float yawMax = Random.Range(_recoilData.yaw.z, _recoilData.yaw.w);

			float yaw = Random.value >= 0.5f ?
										yawMax :
										yawMin;

			float rollMin = Random.Range(_recoilData.roll.x, _recoilData.roll.y);
			float rollMax = Random.Range(_recoilData.roll.z, _recoilData.roll.w);

			float roll = Random.value >= 0.5f ?
										rollMax :
										rollMin;

			roll = (_targetRotation.z * roll) > 0f 
				   && _recoilData.smoothRoll ? 
						-roll : roll;

			float kick = Random.Range(_recoilData.kickback.x, _recoilData.kickback.y);
			float kickRight = Random.Range(_recoilData.kickRight.x, _recoilData.kickRight.y);
			float kickUp = Random.Range(_recoilData.kickUp.x, _recoilData.kickUp.y);

			_noiseTarget.x += Random.Range(_recoilData.noiseX.x, _recoilData.noiseX.y);
			_noiseTarget.y += Random.Range(_recoilData.noiseY.x, _recoilData.noiseY.y);

			_noiseTarget.x *= isAiming ?
				_recoilData.noiseScalar :
				1f;
			
			_noiseTarget.y *= isAiming ?
				_recoilData.noiseScalar :
				1f;

			pitch *= isAiming ?
				_recoilData.aimRotation.x :
				1f;
			
			yaw *= isAiming ?
				_recoilData.aimRotation.y :
				1f;
			
			roll *= isAiming ?
				_recoilData.aimRotation.z :
				1f;

			kick *= isAiming ?
				_recoilData.aimLocation.z :
				1f;
			
			kickRight *= isAiming ?
				_recoilData.aimLocation.x :
				1f;
			
			kickUp *= isAiming ?
				_recoilData.aimLocation.y :
				1f;

			_targetRotation = new Vector3(pitch, yaw, roll);
			_targetLocation = new Vector3(kickRight, kickUp, kick);
		}

		private void UpdateTimeline()
		{
			_playBack += Time.deltaTime * _recoilData.playRate;
			_playBack = Mathf.Clamp(_playBack, 0f, _lastFrameTime);

			// Stop updating if the end is reached
			if (Mathf.Approximately(_playBack, _lastFrameTime) == false)
				return;
			
			if (_isLooping)
			{
				_playBack = 0f;
				_isPlaying = true;
			}
			else
			{
				_isPlaying = false;
				_playBack = 0f;
			}
		}

		private void UpdateSolver()
		{
			if (Mathf.Approximately(_playBack, 0f))
				CalculateTargetData();

			// Current playback position
			float lastPlayback = _playBack - Time.deltaTime * _recoilData.playRate;
			lastPlayback = Mathf.Max(lastPlayback, 0f);

			Vector3 alpha = _tempRotationCurve.Evaluate(_playBack);
			Vector3 lastAlpha = _tempRotationCurve.Evaluate(lastPlayback);

			Vector3 output = Vector3.zero;

			output.x = Mathf.LerpUnclamped(
				CorrectStart(ref lastAlpha.x, alpha.x, ref _canRestRot.x, ref _startValueRotation.x),
				_targetRotation.x, alpha.x);

			output.y = Mathf.LerpUnclamped(
				CorrectStart(ref lastAlpha.y, alpha.y, ref _canRestRot.y, ref _startValueRotation.y),
				_targetRotation.y, alpha.y);

			output.z = Mathf.LerpUnclamped(
				CorrectStart(ref lastAlpha.z, alpha.z, ref _canRestRot.z, ref _startValueRotation.z),
				_targetRotation.z, alpha.z);

			_rawRotationOut = output;

			alpha = _tempLocationCurve.Evaluate(_playBack);
			lastAlpha = _tempLocationCurve.Evaluate(lastPlayback);

			output.x = Mathf.LerpUnclamped(
				CorrectStart(ref lastAlpha.x, alpha.x, ref _canRestLoc.x, ref _startValueLocation.x),
				_targetLocation.x, alpha.x);

			output.y = Mathf.LerpUnclamped(
				CorrectStart(ref lastAlpha.y, alpha.y, ref _canRestLoc.y, ref _startValueLocation.y),
				_targetLocation.y, alpha.y);

			output.z = Mathf.LerpUnclamped(
				CorrectStart(ref lastAlpha.z, alpha.z, ref _canRestLoc.z, ref _startValueLocation.z),
				_targetLocation.z, alpha.z);

			_rawLocationOut = output;
		}

		private void ApplySmoothing()
		{
			if (_isSmoothingEnabled)
			{
				Vector3 lerpedSmoothRotation = _smoothRotationOut;
				Vector3 smooth = _recoilData.smoothRotation;

				float Interp(float a, float b, float speed, float scale)
				{
					scale = Mathf.Approximately(scale, 0f) ?
						1f :
						scale;

					return Mathf.Approximately(speed, 0f) ?
						b * scale :
						CoreToolkitLib.Glerp(a, b * scale, speed);
				}

				lerpedSmoothRotation.x = Interp(_smoothRotationOut.x, _rawRotationOut.x, smooth.x, _recoilData.extraRotation.x);
				lerpedSmoothRotation.y = Interp(_smoothRotationOut.y, _rawRotationOut.y, smooth.y, _recoilData.extraRotation.y);
				lerpedSmoothRotation.z = Interp(_smoothRotationOut.z, _rawRotationOut.z, smooth.z, _recoilData.extraRotation.z);
				
				_smoothRotationOut = lerpedSmoothRotation;

				lerpedSmoothRotation = _smoothLocationOut;
				smooth = _recoilData.smoothLocation;

				lerpedSmoothRotation.x = Interp(_smoothLocationOut.x, _rawLocationOut.x, smooth.x, _recoilData.extraLocation.x);
				lerpedSmoothRotation.y = Interp(_smoothLocationOut.y, _rawLocationOut.y, smooth.y, _recoilData.extraLocation.y);
				lerpedSmoothRotation.z = Interp(_smoothLocationOut.z, _rawLocationOut.z, smooth.z, _recoilData.extraLocation.z);

				_smoothLocationOut = lerpedSmoothRotation;
			}
			else
			{
				_smoothRotationOut = _rawRotationOut;
				_smoothLocationOut = _rawLocationOut;
			}
		}

		private void ApplyNoise(ref Vector3 finalized)
		{
			_noiseTarget.x = CoreToolkitLib.Glerp(_noiseTarget.x, 0f, _recoilData.noiseDamp.x);
			_noiseTarget.y = CoreToolkitLib.Glerp(_noiseTarget.y, 0f, _recoilData.noiseDamp.y);

			_noiseOut.x = CoreToolkitLib.Glerp(_noiseOut.x, _noiseTarget.x, _recoilData.noiseAccel.x);
			_noiseOut.y = CoreToolkitLib.Glerp(_noiseOut.y, _noiseTarget.y, _recoilData.noiseAccel.y);

			finalized += new Vector3(_noiseOut.x, _noiseOut.y, 0f);
		}

		private void ApplyPushback(ref Vector3 finalized)
		{
			_pushTarget = CoreToolkitLib.Glerp(_pushTarget, 0f, _recoilData.pushDamp);
			_pushOut = CoreToolkitLib.Glerp(_pushOut, _pushTarget, _recoilData.pushAccel);

			finalized += new Vector3(0f, 0f, _pushOut);
		}

		private float CorrectStart(ref float last, float current, ref bool isStartRest, ref float startValue)
		{
			if (Mathf.Abs(last) > Mathf.Abs(current) 
								&& isStartRest 
								&& !_isLooping)
			{
				startValue = 0f;
				isStartRest = false;
			}

			last = current;

			return startValue;
		}

		private void SetupStateMachine()
		{
			_animationStateMachine ??= new List<AnimState>();

			AnimState semiState;
			AnimState autoState;

			semiState.CheckCondition = () =>
			{
				float timerError = 60f / _fireRate / Time.deltaTime + 1;
				timerError *= Time.deltaTime;

				if (_isSmoothingEnabled && !_isLooping)
					_isSmoothingEnabled = false;

				return (Delta > timerError + 0.01f && !_isLooping) || fireMode == FireMode.Semi;
			};

			semiState.OnPlay = () =>
			{
				SetupTransition(_smoothRotationOut, 
							_smoothLocationOut, 
							_recoilData.recoilCurves.semiRotationCurve,
							_recoilData.recoilCurves.semiLocationCurve);
			};

			semiState.OnStop = () =>
			{
				//Intended to be empty
			};

			autoState.CheckCondition = () => true;

			autoState.OnPlay = () =>
			{
				if (_isLooping)
					return;

				RecoilCurves curves = _recoilData.recoilCurves;
				
				bool bCurvesValid = curves.autoRotationCurve.IsValid 
								  && curves.autoLocationCurve.IsValid;

				_isSmoothingEnabled = bCurvesValid;
				float correction = 60f / _fireRate;

				if (bCurvesValid)
				{
					CorrectAlpha(curves.autoRotationCurve, curves.autoLocationCurve, correction);
					SetupTransition(_startValueRotation, _startValueLocation, curves.autoRotationCurve, curves.autoLocationCurve);
				}
				else if (curves.autoRotationCurve.IsValid && curves.autoLocationCurve.IsValid)
				{
					CorrectAlpha(curves.semiRotationCurve, curves.semiLocationCurve, correction);
					SetupTransition(_startValueRotation, _startValueLocation, curves.semiRotationCurve, curves.semiLocationCurve);
				}

				_pushTarget = _recoilData.pushAmount;

				_lastFrameTime = correction;
				_isLooping = true;
			};

			autoState.OnStop = () =>
			{
				if (!_isLooping)
					return;

				float tempRot = _tempRotationCurve.LastKeyframeTime;
				float tempLoc = _tempLocationCurve.LastKeyframeTime;
				_lastFrameTime = tempRot > tempLoc ?
											tempRot :
											tempLoc;
				_isPlaying = true;
			};

			_animationStateMachine.Add(semiState);
			_animationStateMachine.Add(autoState);
		}

		private void SetupTransition(Vector3 startRot,
									 Vector3 startLoc,
									 Vector3DCurve rotationCurve,
									 Vector3DCurve locationCurve)
		{
			if (!rotationCurve.IsValid || !locationCurve.IsValid)
			{
				Debug.Log("RecoilAnimation: Rot or Loc curve is nullptr");
				return;
			}

			_startValueRotation = startRot;
			_startValueLocation = startLoc;

			_canRestRot = _canRestLoc = new StartRest(true, true, true);

			_tempRotationCurve = rotationCurve;
			_tempLocationCurve = locationCurve;

			_lastFrameTime = rotationCurve.LastKeyframeTime > locationCurve.LastKeyframeTime ?
															rotationCurve.LastKeyframeTime :
															locationCurve.LastKeyframeTime;

			PlayFromStart();
		}

		private void CorrectAlpha(Vector3DCurve rotation, Vector3DCurve location, float time)
		{
			Vector3 curveAlpha = rotation.Evaluate(time);

			_startValueRotation.x = Mathf.LerpUnclamped(_startValueRotation.x, _targetRotation.x, curveAlpha.x);
			_startValueRotation.y = Mathf.LerpUnclamped(_startValueRotation.y, _targetRotation.y, curveAlpha.y);
			_startValueRotation.z = Mathf.LerpUnclamped(_startValueRotation.z, _targetRotation.z, curveAlpha.z);

			curveAlpha = location.Evaluate(time);

			_startValueLocation.x = Mathf.LerpUnclamped(_startValueLocation.x, _targetLocation.x, curveAlpha.x);
			_startValueLocation.y = Mathf.LerpUnclamped(_startValueLocation.y, _targetLocation.y, curveAlpha.y);
			_startValueLocation.z = Mathf.LerpUnclamped(_startValueLocation.z, _targetLocation.z, curveAlpha.z);
		}

		private void PlayFromStart()
		{
			_playBack = 0f;
			_isPlaying = true;
		}
	}
}