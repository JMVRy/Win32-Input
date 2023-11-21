<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->
<a name="readme-top"></a>
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Don't forget to give the project a star!
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]



<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/JMVRy/Win32-Input">
    <img src="images/Logo.png" alt="Logo" width="80" height="80">
  </a>

<h3 align="center">Win32 Input</h3>

  <p align="center">
    A tool to cheat at the <a href="https://humanbenchmark.com/tests/aim">Aim Trainer on HumanBenchmark.com</a>
    <br />
    <a href="https://github.com/JMVRy/Win32-Input/wiki/Documentation"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="#usage">View Demo</a>
    ·
    <a href="https://github.com/JMVRy/Win32-Input/issues">Report Bug</a>
    ·
    <a href="https://github.com/JMVRy/Win32-Input/issues">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

This project was made to automate various inputs using Windows' Win32 API. It was created out of necessity because I couldn't find a NuGet package that suited my needs, and I just didn't really care to look that hard for one, so I created my own. 

<p align="right">(<a href="#readme-top">back to top</a>)</p>



### Built With

* [![C#][CSharp]][CSharp-url]
* [![Windows][Windows]][Windows-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

To run the program on your own machine, follow the following steps.

### Prerequisites

This is a list on all the things you need, in order to run the software on your own machine
* Windows 7/8/10/11/Above
  * Any Windows version should work, but I'd suggest 7 or above, because I can't be certain if any version below 7 will work.

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/JMVRy/Win32-Input.git
   ```
1. Use the input classes
   ```cs
   using JMVR;
   
   // Moves mouse to 0, 0 (top-left)
   MouseOperations.SetCursorPosition(0, 0);
   
   // Types "hello"
   InputOperations.SendUnicode("hello");
   
   // Presses the F24 key (not found on most keyboards, relic of old keyboard used for compatibility sake)
   InputOperations.SendKeypress(InputOperations.VirtualKeyShort.VK_F24);
   ```

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->
## Usage

This project was primarily made for allowing me to do input automation for simple projects. If you've followed the Installation setup and there's no more problems, then it should just be plug-and-play. Use with your own code, and it should do what you want it to do. Some confusion may occur regarding multiple monitor setups, but that's because Win32 is kinda weird with those, and I don't really know how else it should be done.

_For more information, please refer to the [Documentation](https://github.com/JMVRy/Win32-Input/wiki/Documentation)_

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/JMVRy/Win32-Input/issues) for a full list of proposed features (and known issues).

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- LICENSE -->
## License

Distributed under the GNU General Public License Version 3. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

JohnMarc Everly Jr. - [LinkedIn][linkedin-url] - srjr18@gmail.com

Project Link: [https://github.com/JMVRy/Win32-Input](https://github.com/JMVRy/Win32-Input)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* [Microsoft for the Operating System](https://microsoft.com)
* [Windows for the API](https://microsoft.com/en-us/windows)
* [Microsoft again for their documentation being <sup>_reasonably_</sup> well-made](https://learn.microsoft.com/en-us/windows/win32/api/)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<hr />

<sup>This README was made with the [Best README Template repository](https://github.com/othneildrew/Best-README-Template).</sup>



<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/JMVRy/Win32-Input.svg?style=for-the-badge
[contributors-url]: https://github.com/JMVRy/Win32-Input/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/JMVRy/Win32-Input.svg?style=for-the-badge
[forks-url]: https://github.com/JMVRy/Win32-Input/network/members
[stars-shield]: https://img.shields.io/github/stars/JMVRy/Win32-Input.svg?style=for-the-badge
[stars-url]: https://github.com/JMVRy/Win32-Input/stargazers
[issues-shield]: https://img.shields.io/github/issues/JMVRy/Win32-Input.svg?style=for-the-badge
[issues-url]: https://github.com/JMVRy/Win32-Input/issues
[license-shield]: https://img.shields.io/github/license/JMVRy/Win32-Input.svg?style=for-the-badge
[license-url]: https://github.com/JMVRy/Win32-Input/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/johnmarc-everly-jr-882021225

[ScreenCapture.NET]: https://www.nuget.org/packages/ScreenCapture.NET
[ScreenCapture.NET.DX11]: https://www.nuget.org/packages/ScreenCapture.NET.DX11

[product-screenshot]: images/Screenshot.png

<!-- Product images and URLs -->
[Next.js]: https://img.shields.io/badge/next.js-000000?style=for-the-badge&logo=nextdotjs&logoColor=white
[Next-url]: https://nextjs.org/
[React.js]: https://img.shields.io/badge/React-20232A?style=for-the-badge&logo=react&logoColor=61DAFB
[React-url]: https://reactjs.org/
[Vue.js]: https://img.shields.io/badge/Vue.js-35495E?style=for-the-badge&logo=vuedotjs&logoColor=4FC08D
[Vue-url]: https://vuejs.org/
[Angular.io]: https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white
[Angular-url]: https://angular.io/
[Svelte.dev]: https://img.shields.io/badge/Svelte-4A4A55?style=for-the-badge&logo=svelte&logoColor=FF3E00
[Svelte-url]: https://svelte.dev/
[Laravel.com]: https://img.shields.io/badge/Laravel-FF2D20?style=for-the-badge&logo=laravel&logoColor=white
[Laravel-url]: https://laravel.com
[Bootstrap.com]: https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white
[Bootstrap-url]: https://getbootstrap.com
[JQuery.com]: https://img.shields.io/badge/jQuery-0769AD?style=for-the-badge&logo=jquery&logoColor=white
[JQuery-url]: https://jquery.com 
[CSharp]: https://img.shields.io/badge/csharp-512BD4?style=for-the-badge&logo=csharp&color=512BD4
[CSharp-url]: https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/
[Windows]: https://img.shields.io/badge/windows-0078D4?style=for-the-badge&logo=windows&color=0078D4
[Windows-url]: https://microsoft.com/en-us/windows

[repo-url]: https://github.com/JMVRy/Win32-Input

[trainer-hb]: https://humanbenchmark.com/tests/aim
