using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase;
using UnityEngine.UI;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginData : MonoBehaviour
{
    //Đăng ký tài khoản
    [Header("Register")]
    public InputField emailField;
    public InputField passwordField;

    public Button registerButton;

    //Đăng nhâp tài khoản
    [Header("Login")]
    public InputField emailLoginField;
    public InputField passwordLoginField;

    public Button loginButton;

    //Chuyển register <-> login
    [Header("Switch")]
    public Button switchButtonToLogin;
    public Button switchButtonToRegister;

    [Header("Note")]
    public GameObject noteLoginFail;
    public GameObject noteRegisterFail;

    [Header("Button")]
    public GameObject login;
    public GameObject logout;
    public GameObject play;
    public GameObject quit;
    public GameObject loginUI;

    public GameObject loginForm;
    public GameObject registerForm;


    protected FirebaseAuth auth;
    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    private void Start() {
        noteLoginFail.SetActive(false);
        noteRegisterFail.SetActive(false);
        logout.SetActive(false);
        play.SetActive(false);
        loginUI.SetActive(false);

        registerButton.onClick.AddListener(RegisterAccount);
        loginButton.onClick.AddListener(LoginAccount);

        switchButtonToLogin.onClick.AddListener(Switch);
        switchButtonToRegister.onClick.AddListener(Switch);
    }

    public void RegisterAccount()
    {
        string email = emailField.text;
        string password = passwordField.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if(task.IsCanceled)
            {
                Debug.Log("đăng ký bị hủy.");
                return;
            }
            if(task.IsFaulted)
            {
                Debug.Log("đăng ký thất bại.");
                noteRegisterFail.SetActive(true);
                return;
            }
            if(task.IsCompleted)
            {
                noteRegisterFail.SetActive(true);
                noteRegisterFail.GetComponent<Text>().text = "Successful registration! Proceed to the login screen!";
                Debug.Log("đăng ký thành công.");
            }
        });
    }

    public void LoginAccount()
    {
        string email = emailLoginField.text;
        string password = passwordLoginField.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if(task.IsCanceled)
            {
                Debug.Log("đăng nhập bị hủy.");
                return;
            }
            if(task.IsFaulted)
            {
                Debug.Log("đăng nhập thất bại.");
                noteLoginFail.SetActive(true);
                return;
            }
            if(task.IsCompleted)
            {
                Debug.Log("đăng nhập thành công.");
                FirebaseUser user = task.Result.User;
                emailLoginField.text = "";
                passwordLoginField.text = "";
                loginUI.SetActive(false);
                login.SetActive(false);
                logout.SetActive(true);
                play.SetActive(true);
            }
        });
    }
    public void Play(){
        SceneManager.LoadScene("SampleScene");
    }

    public void Login(){
        loginUI.SetActive(true);
    }
    public void Logout(){
        auth.SignOut();
        play.SetActive(false);
        logout.SetActive(false);
        login.SetActive(true);
    }
    public void Quit(){
        Application.Quit();
    }

    public void Switch()
    {
        loginForm.SetActive(!loginForm.activeSelf);
        registerForm.SetActive(!registerForm.activeSelf);
    }
}
