//-----------------------------------------------------------------------
// <copyright file="MeshBuilderWithColorGUIController.cs" company="Google">
//
// Copyright 2016 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using Tango;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Extra GUI controls.
/// </summary>
public class MeshBuilderWithColorGUIController : MonoBehaviour
{
    /// <summary>
    /// Debug info: If the mesh is being updated.
    /// </summary>
    private bool m_isEnabled = true;
    private bool showContinue = false;
    private bool showDelete = false;
    private bool showSave = false;


    private TangoApplication m_tangoApplication;
    private TangoDynamicMesh m_dynamicMesh;
    private Exporter m_exporter;
    private TangoMultiCamera m_tangoCamera;
    public GameObject measurePointStart;
    public GameObject measurePointEnd;
    private Camera mainCamera;
    private Vector3 pos;
    private TangoDeltaPoseController m_poseController;
    private float dist;

    public Texture pause;
	public Texture resume;
	public Texture clear;
	public Texture save;
    public Texture circle;
    public Texture start;
    public Texture end;
    public Texture delete;
    public Texture cancel;
    public Texture blank;
    public Texture continueMessage;
    public Texture continueButton;
    public Texture backArrow;
    public Texture saveMessage;
    public Texture deleteMessage;
    public Texture recordingMenu;
    
	public GUIStyle material;
    public string distanceOut;


    /// <summary>
    /// Start is used to initialize.
    /// </summary>
    public void Start()
    {
        m_tangoApplication = FindObjectOfType<TangoApplication>();
        m_dynamicMesh = FindObjectOfType<TangoDynamicMesh>();
        m_exporter = new Exporter();
        m_tangoCamera = FindObjectOfType<TangoMultiCamera>();
        m_poseController = FindObjectOfType<TangoDeltaPoseController>();
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        Material lineMat = Resources.Load("Line", typeof(Material)) as Material;
        lineRenderer.material = lineMat;
        lineRenderer.SetWidth(0.01F, 0.01F);
        lineRenderer.SetVertexCount(2);
        lineRenderer.SetPosition(0, measurePointStart.transform.position);
        lineRenderer.SetPosition(1, measurePointEnd.transform.position);
    }

    /// <summary>
    /// Updates UI and handles player input.
    /// </summary>
    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (measurePointStart.transform.position.y != -22 && measurePointEnd.transform.position.y != -22)
        {
            lineRenderer.SetPosition(0, measurePointStart.transform.position);
            lineRenderer.SetPosition(1, measurePointEnd.transform.position);
        }
    }

    /// <summary>
    /// Draws the Unity GUI.
    /// </summary>
    public void OnGUI()
    {   
        //Menu Bar
        GUI.Label(new Rect(0, -10, Screen.width, Screen.height), recordingMenu);
        if (GUI.Button(new Rect(10, 10, 75, 75), backArrow, material))
        {
            showContinue = true;
        }
        if(showContinue)
        {
            m_tangoApplication.Set3DReconstructionEnabled(false);
            GUI.Label(new Rect((Screen.width / 2) - 480, (Screen.height / 2) - 300, 960, 600), continueMessage);
            if (GUI.Button(new Rect((Screen.width / 2) - 175, (Screen.height / 2) + 100, 200, 100), continueButton, material))
            {
                SceneManager.LoadScene(0);
            }
            if (GUI.Button(new Rect((Screen.width / 2) + 75, (Screen.height / 2) + 100, 200, 100), cancel, material))
            {
                showContinue = false;
            }
        }
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        
		//Delete button
        if (GUI.Button(new Rect(60, Screen.height - 200, 140, 140), clear, material))
		{
            showDelete = true;
        }
        if (showDelete)
        {
            m_tangoApplication.Set3DReconstructionEnabled(false);
            GUI.Label(new Rect((Screen.width / 2) - 480, (Screen.height / 2) - 300, 960, 600), deleteMessage);
            if (GUI.Button(new Rect((Screen.width / 2) - 175, (Screen.height / 2) + 100, 200, 100), delete, material))
            {
                m_dynamicMesh.Clear();
                m_tangoApplication.Tango3DRClear();
                measurePointStart.transform.position = new Vector3(0, -22, 0);
                measurePointEnd.transform.position = new Vector3(0, -22, 0);
                lineRenderer.SetPosition(0, measurePointStart.transform.position);
                lineRenderer.SetPosition(1, measurePointEnd.transform.position);
                dist = 0;
                distanceOut = null;
                GUI.Label(new Rect(Screen.width - 360, 420, 340, 50), "");
                showDelete = false;
            }
            if (GUI.Button(new Rect((Screen.width / 2) + 75, (Screen.height / 2) + 100, 200, 100), cancel, material))
            {
                showDelete = false;
            }
        }


        Texture text = m_isEnabled ? pause : resume;
		if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 200, 140, 140), text, material))
		{
			m_isEnabled = !m_isEnabled;
			m_tangoApplication.Set3DReconstructionEnabled(m_isEnabled);
		}


        if (!m_isEnabled)
        {
            GUI.Button(new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20), circle, material);

            // Save Button to show the message
            if (GUI.Button(new Rect(Screen.width - 400, Screen.height - 200, 140, 140), save, material))
			{
                showSave = true;
            }
				
            // Start Point Button
            if (GUI.Button(new Rect(Screen.width - 160, 180, 140, 140), start, material))
            {
                setStartPoint();
            }

            // End Point Button
            if (GUI.Button(new Rect(Screen.width - 160, 380, 140, 140), end, material))
            {
                setEndPoint();
            }
        
		}

        if (measurePointStart.transform.position.y != -22 && measurePointEnd.transform.position.y != -22)
        {

            dist = Vector3.Distance(measurePointStart.transform.position, measurePointEnd.transform.position);
            double inches, inc;
            int feet;
            inc = dist * 39.3701;
            inches = inc % 12;
            feet = (int)inc / 12;

            GUI.color = Color.black;
            GUI.contentColor = Color.gray;
            
            int measurement = PlayerPrefs.GetInt("Feet");
            if (measurement == 1)
            {
                GUI.Label(new Rect(Screen.width - 450, 620, 450, 100), string.Format("<size=70>{0} ft {1} in</size>", feet.ToString(), inches.ToString("n2")));
            }
            if (measurement == 0)
            {
                GUI.Label(new Rect(Screen.width - 450, 620, 450, 100), string.Format("<size=70>{0} meters</size>", dist.ToString("n3")));
            }
				
        }

		if (showSave)
		{
			GUI.Label(new Rect((Screen.width / 2) - 480, (Screen.height / 2) - 300, 960, 600), saveMessage);

			Invoke ("saveMesh", 1);
		}
    }

    public void saveMesh()
    {
		showSave = false;
		m_exporter.DoExport(true);
		CancelInvoke ("saveMesh");
    }

    public void setStartPoint()
    {
        Vector3 startpoint = Vector3.zero;
        var startray = new Ray(m_poseController.m_tangoPosition, m_poseController.transform.forward);
        RaycastHit hit; // declare the RaycastHit variable
        if (Physics.Raycast(startray, out hit))
        {
            startpoint = hit.point;
            measurePointStart.transform.position = startpoint;
            measurePointStart.transform.rotation = m_poseController.m_tangoRotation;
        }
    }

    public void setEndPoint()
    {
        Vector3 endpoint = Vector3.zero;
        var endray = new Ray(m_poseController.m_tangoPosition, m_poseController.transform.forward);
        RaycastHit hit; // declare the RaycastHit variable
        if (Physics.Raycast(endray, out hit))
        {
            endpoint = hit.point;
            measurePointEnd.transform.position = endpoint;
            measurePointStart.transform.rotation = m_poseController.m_tangoRotation;
        }
    }

}
