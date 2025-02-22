# TETR.IO Overlay

A simple overlay for displaying your Tetra League, Quick Play, 40L, Blitz and much more stats in OBS using a Browser Source. You can choose to display your stats as a static image or a live view that updates every 30 seconds.

If you find this overlay useful, please consider giving it a ⭐ to show your support, or share it with others who might enjoy it. Thank you for using it! It really means a lot to me 🧡🦊

> *this project also contains the backend of the [Zenith Daily Challenge](https://github.com/Founntain/Zenith.DailyChallenge.Web) as this project contains almost like 99% of the code that is need anyways*

## 🎖️ Usage

For screenshots and examples look at the [examples section](#examples) at the bottom.  
To use the overlay, simply use one of the following URLs:

### Tetra League
- **Live View**: `https://tetrio.founntain.dev/user/<username>`

### Tetra League
- **Live View**: `https://tetrio.founntain.dev/tetraleague/<username>`

### Quick Play
- **Live View**: `https://tetrio.founntain.dev/zenith/<username>`

### 40 Lines
- **Live View**: `https://tetrio.founntain.dev/sprint/<username>`

### Blitz
- **Live View**: `https://tetrio.founntain.dev/blitz/<username>`

### QP Speedrun Splits
- **Live View**: `https://tetrio.founntain.dev/zenith/splits/<username>`
> for more info check the [splits section](#Splits).

### 📽️ OBS Setup

For OBS, it is recommended to use the live view URL. To set it up:

1. Create a new Browser Source in OBS.
2. Paste the live view URL into the Browser Source settings, replacing `<username>` with your tetr.io username (make sure to remove the `<` and `>`), the same goes for `<mode>`.
3. Make sure the width and height is correct check below what sizes are best for each overlay:
   - User Card: 800 x 350
   - Tetra League: 800 x 350
   - Quick Play: 900 x 350
   - 40 Lines: 700 x 225
   - Blitz: 700 x 225
   - Speedrun splits: 1500 x 200

> [!NOTE]  
> The data is cached and refreshes every 30 seconds. For 40 Lines and Blitz the default cache is used which is **5 minutes**.

### 🛠️ Customization Parameters
You can customize a lot, as they are all web-based you can modify the CSS to your liking in OBS.
Here are some common examples for the **user card**:
```CSS
// Hide the profile picture
.profilePicture{ display: none }

// Hide Tetra League Progressbar
#tetraLeagueProgressContainer { display: none }

// Change the text color for example red
.body{ color: #FF0000 }

// Hide 40L, Blitz or QP
#sprintContainer { display: none }
#blitzContainer { display: none }
#zenithContainer { display: none }

// Or hide them all together with one line
.gamemodesContainer { display: none }
```

## Splits

Ever wanted to know how fast you clear floors without hitting hyperspeed, or not able to hit hyperspeed ever, to see the splits in general? We have you covered. With the splits overlay, you can compare your times with your gold splits from this week.  
The splits overlay uses your last 100 games from the current week; all games past that aren't counted. We chose this limit because we don't want to make excessive API calls, and the last 100 games is enough to compare your performance this week. If you haven't played this week yet, we use your career best splits for display.  
With the `expert` parameter you can show the splits for expert or normal, default value is `false`

![image](https://github.com/user-attachments/assets/5f20844a-9fef-4559-a6b6-48e8852b7ebe)

> [!CAUTION]
> The splits overlay is still very early and pretty complex. If something does not work as intended, or is not working at all, please let me know so I can look into it!  
> ***The splits overlay is not supported in slide mode, because it is quite large. Maybe in the future.***

## 🏠 Running Locally

If you prefer not to use the hosted version, you have a few alternatives:

- Clone the repository, build, and run the project locally, then access it via `localhost`.
- Pull the [Docker image](https://hub.docker.com/repository/docker/founntain/tetrio.overlay.api/general) and run it with Docker, accessing it via `localhost` and the assigned port.

## 🔨 Contributing

Contributions are welcome! Feel free to open issues, request features, provide feedback, report bugs, or even contribute code. If you're unsure about anything, let's discuss it. Thanks for your support!

## 🧡Special Thanks

- **[osk](https://tetr.io)**: for creating tetr.io and providing an amazing, well-structured API.
  - *all assets like rank and mod icons belong to osk and tetr.io*
- **[Veggie_Dog](https://www.twitch.tv/theveggiedog)**: motivating to make this project a reality, testing and feedback
- **[PixelAtc](https://www.twitch.tv/pixelatc)**: providing feedback, ideas and spreading the word
- **[ZaptorZap](https://zaptorz.app/)**: for giving feedback and some incredible ideas

## Examples
> *added a background to all of them so they are visible on github light mode users*
>

#### User Card
![image](https://github.com/user-attachments/assets/f873cb7e-8917-422f-bf90-93160ca53cb8)

#### Tetra League
![founntain](https://github.com/user-attachments/assets/b867218b-de57-4a44-85d3-1a5721878720)

#### Quick Play
![image](https://github.com/user-attachments/assets/e91f4eb6-0fd8-4b9a-aa6c-8f0fdce8a43d)

#### Speedrun splits
![image](https://github.com/user-attachments/assets/314b1ab6-98b7-4a5f-aad7-aa447ceb7f2b)

#### Sprint
![image](https://github.com/user-attachments/assets/1eaae399-546e-470a-b108-360d5a34818b)

#### Blitz
![image](https://github.com/user-attachments/assets/90fc37f2-2a7d-408b-a60a-f412e38c3378)

