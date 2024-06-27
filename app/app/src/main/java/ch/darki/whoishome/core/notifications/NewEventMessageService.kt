package ch.darki.whoishome.core.notifications

import android.util.Log
import androidx.core.app.NotificationCompat
import ch.darki.whoishome.R
import com.google.firebase.messaging.FirebaseMessagingService
import com.google.firebase.messaging.RemoteMessage

class NewEventMessageService : FirebaseMessagingService() {

    override fun onNewToken(token: String) {
        super.onNewToken(token)
        Log.d("NewEventMessageService", "Refreshed token: $token")
    }

    override fun onMessageReceived(remoteMessage: RemoteMessage) {
        super.onMessageReceived(remoteMessage)
        Log.d("NewEventMessageService", "Message data: ${remoteMessage.data}")

        // TODO filter if Notification is relevant

        // notify(remoteMessage)
    }

    private fun notify(remoteMessage: RemoteMessage) {
        var builder = NotificationCompat.Builder(this, CHANNEL_ID)
            .setSmallIcon(R.drawable.ic_launcher_foreground)
            .setContentTitle("Neues Event!")
            .setContentText("${remoteMessage.data["person-name"]} hat gerade ein neues Event für Heute eingetragen")
            .setPriority(NotificationCompat.PRIORITY_DEFAULT)

        // TODO show notification
    }

    companion object {
        const val CHANNEL_ID = "NewEvent"
    }
}