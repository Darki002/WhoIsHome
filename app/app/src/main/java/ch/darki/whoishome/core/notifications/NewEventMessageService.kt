package ch.darki.whoishome.core.notifications

import android.app.NotificationManager
import android.app.PendingIntent
import android.content.Intent
import android.util.Log
import androidx.core.app.NotificationCompat
import androidx.core.app.NotificationManagerCompat
import androidx.core.app.TaskStackBuilder
import ch.darki.whoishome.R
import ch.darki.whoishome.mainActivity.MainActivity
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

    private fun notify(personName : String) {
        val builder = NotificationCompat.Builder(this, CHANNEL_ID)
            .setContentTitle("Neues Event!")
            .setContentText("$personName hat gerade ein neues Event für Heute eingetragen")
            .setPriority(NotificationCompat.PRIORITY_DEFAULT)
            .setAutoCancel(true)

        val notificationManager = getSystemService(NOTIFICATION_SERVICE) as NotificationManager
        notificationManager.notify(NOTIFICATION_ID, builder.build())
    }

    companion object {
        const val CHANNEL_ID = "NewEvent"
        const val NOTIFICATION_ID = 0
    }
}